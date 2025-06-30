using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;

namespace CashFlow.Shared.Exceptions.Application
{
    public class DatabaseInitializationException : AppException
    {
        public DatabaseInitializationException(string message, Exception? inner = null)
            : base(message, StatusCodes.Status500InternalServerError)
        {
        }
    }
}
