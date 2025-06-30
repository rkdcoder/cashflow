using CashFlow.Shared.Utilities;

namespace CashFlow.Shared.Tests.Utilities
{
    public class InputValidatorServiceTests
    {
        private readonly InputValidatorService _service = new();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void ValidateEmail_Should_Return_Failure_If_Empty(string? email)
        {
            var result = _service.ValidateEmail(email!);
            Assert.False(result.Success);
            Assert.Contains("required", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("user@domain")]
        [InlineData("@domain.com")]
        [InlineData("user@.com")]
        public void ValidateEmail_Should_Return_Failure_If_Invalid_Format(string email)
        {
            var result = _service.ValidateEmail(email);
            Assert.False(result.Success);
            Assert.Contains("format", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }


        [Theory]
        [InlineData("user@test.com")]
        [InlineData(" User@Domain.COM ")]
        public void ValidateEmail_Should_Return_Success_If_Valid(string email)
        {
            var result = _service.ValidateEmail(email);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateLoginName_Should_Return_Failure_If_Empty(string? login)
        {
            var result = _service.ValidateLoginName(login!);
            Assert.False(result.Success);
            Assert.Contains("required", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("ab")]
        [InlineData("a")]
        public void ValidateLoginName_Should_Return_Failure_If_Too_Short(string login)
        {
            var result = _service.ValidateLoginName(login);
            Assert.False(result.Success);
            Assert.Contains("at least 3", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("abc@def")]
        [InlineData("user@")]
        public void ValidateLoginName_Should_Return_Failure_If_Contains_At(string login)
        {
            var result = _service.ValidateLoginName(login);
            Assert.False(result.Success);
            Assert.Contains("not contain the '@'", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("abc123")]
        [InlineData("user_test")]
        [InlineData("us3r")]
        [InlineData("User!")]
        public void ValidateLoginName_Should_Return_Failure_If_Not_Only_Letters(string login)
        {
            var result = _service.ValidateLoginName(login);
            Assert.False(result.Success);
            Assert.Contains("only letters", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("userName")]
        [InlineData("ABC")]
        [InlineData("Username")]
        public void ValidateLoginName_Should_Return_Success_If_Valid(string login)
        {
            var result = _service.ValidateLoginName(login);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateUserName_Should_Return_Failure_If_Empty(string? name)
        {
            var result = _service.ValidateUserName(name!);
            Assert.False(result.Success);
            Assert.Contains("required", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("ab")]
        [InlineData("a")]
        public void ValidateUserName_Should_Return_Failure_If_Too_Short(string name)
        {
            var result = _service.ValidateUserName(name);
            Assert.False(result.Success);
            Assert.Contains("at least 3", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("Name123")]
        [InlineData("User_One")]
        [InlineData("User!")]
        public void ValidateUserName_Should_Return_Failure_If_Invalid_Characters(string name)
        {
            var result = _service.ValidateUserName(name);
            Assert.False(result.Success);
            Assert.Contains("only letters", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("John Doe")]
        [InlineData("Alice Bob")]
        [InlineData("Username")]
        public void ValidateUserName_Should_Return_Success_If_Valid(string name)
        {
            var result = _service.ValidateUserName(name);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void ValidatePassword_Should_Return_Failure_If_Empty(string? pwd)
        {
            var result = _service.ValidatePassword(pwd!);
            Assert.False(result.Success);
            Assert.Contains("required", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("Abc123")] // too short
        [InlineData("A234567")] // too short
        [InlineData("AbcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890AbcdefghijklmnopqrstuvwxyzAB")] // 73 chars
        public void ValidatePassword_Should_Return_Failure_If_Length_Invalid(string pwd)
        {
            var result = _service.ValidatePassword(pwd);
            Assert.False(result.Success);
            Assert.Contains("between 8 and 72", result.Message, System.StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("Password1")]
        [InlineData("A1b2c3d4")]
        [InlineData("Abc12345")]
        public void ValidatePassword_Should_Return_Success_If_Valid(string pwd)
        {
            var result = _service.ValidatePassword(pwd);
            Assert.True(result.Success);
        }
    }
}
