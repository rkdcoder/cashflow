using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;

namespace CashFlow.Shared.Exceptions.Application
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, StatusCodes.Status400BadRequest)
        {
        }
    }
}