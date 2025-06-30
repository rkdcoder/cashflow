using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;

namespace CashFlow.Shared.Exceptions.Application
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound)
        {
        }
    }
}