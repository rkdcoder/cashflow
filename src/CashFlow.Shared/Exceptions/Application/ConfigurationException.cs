using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;

namespace CashFlow.Shared.Exceptions.Application
{
    public class ConfigurationException : AppException
    {
        public ConfigurationException(string message)
            : base(message, StatusCodes.Status500InternalServerError)
        {
        }
    }
}