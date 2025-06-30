using CashFlow.Shared.Utilities;

namespace CashFlow.Shared.Tests.Utilities
{
    public class UserHierarchyUtilsTests
    {
        [Theory]
        [InlineData("USER", 1)]
        [InlineData("MANAGER", 2)]
        [InlineData("ADMIN", 3)]
        [InlineData("UNKNOWN", 0)]
        [InlineData("", 0)]
        public void GetRoleLevel_Should_Return_Correct_Level(string role, int expected)
        {
            var level = UserHierarchyUtils.GetRoleLevel(role);
            Assert.Equal(expected, level);
        }

        [Fact]
        public void GetHighestRoleLevel_Should_Return_Max_Level()
        {
            var roles = new List<string> { "USER", "MANAGER", "ADMIN" };
            var level = UserHierarchyUtils.GetHighestRoleLevel(roles);
            Assert.Equal(3, level);
        }

        [Fact]
        public void GetHighestRoleLevel_Should_Return_Zero_For_Unknown_Or_Null()
        {
#nullable disable
            Assert.Equal(0, UserHierarchyUtils.GetHighestRoleLevel(null));
#nullable enable
            Assert.Equal(0, UserHierarchyUtils.GetHighestRoleLevel(new List<string>()));
            Assert.Equal(0, UserHierarchyUtils.GetHighestRoleLevel(new List<string> { "FOO", "BAR" }));
        }

        [Theory]
        [InlineData(new[] { "USER" }, "USER", true)]
        [InlineData(new[] { "USER" }, "MANAGER", false)]
        [InlineData(new[] { "MANAGER" }, "USER", true)]
        [InlineData(new[] { "ADMIN" }, "MANAGER", true)]
        [InlineData(new[] { "ADMIN" }, "ADMIN", true)]
        [InlineData(new[] { "USER", "MANAGER" }, "ADMIN", false)]
        [InlineData(new string[] { }, "USER", false)]
        public void HasRequiredRole_Should_Work_Correctly(string[] userRoles, string requiredRole, bool expected)
        {
            var result = UserHierarchyUtils.HasRequiredRole(userRoles, requiredRole);
            Assert.Equal(expected, result);
        }
    }
}