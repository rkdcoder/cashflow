using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;

namespace CashFlow.Shared.Exceptions.Application
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            : base(message, StatusCodes.Status403Forbidden)
        {
        }
    }
}