using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;

namespace CashFlow.Shared.Exceptions.Application
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, StatusCodes.Status401Unauthorized)
        {
        }
    }
}
