using CashFlow.Infrastructure.Shared.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CashFlow.Infrastructure.Shared.Tests.Identity
{
    public class CurrentUserContextTests
    {
        [Fact]
        public void Should_Return_Claims_From_HttpContext()
        {
            // Arrange
            var subject = "user-123";
            var email = "john@cashflow.com";
            var name = "John Doe";
            var roles = new[] { "Admin", "User" };

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, subject),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, roles[0]),
                new Claim(ClaimTypes.Role, roles[1])
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext();
            context.User = principal;

            var accessor = new HttpContextAccessor { HttpContext = context };

            var currentUserContext = new CurrentUserContext(accessor);

            // Act & Assert
            Assert.Equal(subject, currentUserContext.Subject);
            Assert.Equal(email, currentUserContext.Email);
            Assert.Equal(name, currentUserContext.Name);
            Assert.Equal(2, currentUserContext.Roles.Count);
            Assert.Contains("Admin", currentUserContext.Roles);
            Assert.Contains("User", currentUserContext.Roles);
        }

        [Fact]
        public void Should_Return_Null_And_Empty_When_HttpContext_Or_User_Is_Null()
        {
            // Arrange
            var accessor = new HttpContextAccessor { HttpContext = null };
            var currentUserContext = new CurrentUserContext(accessor);

            // Act & Assert
            Assert.Null(currentUserContext.Subject);
            Assert.Null(currentUserContext.Email);
            Assert.Null(currentUserContext.Name);
            Assert.Empty(currentUserContext.Roles);

            // Now test with User set to null
            var context = new DefaultHttpContext();
            context.User = null!;
            accessor.HttpContext = context;

            Assert.Null(currentUserContext.Subject);
            Assert.Null(currentUserContext.Email);
            Assert.Null(currentUserContext.Name);
            Assert.Empty(currentUserContext.Roles);
        }

        [Fact]
        public void Should_Return_Empty_Roles_If_No_Role_Claims()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-321")
                // no role claims
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext();
            context.User = principal;

            var accessor = new HttpContextAccessor { HttpContext = context };

            var currentUserContext = new CurrentUserContext(accessor);

            // Act & Assert
            Assert.Empty(currentUserContext.Roles);
        }
    }
}