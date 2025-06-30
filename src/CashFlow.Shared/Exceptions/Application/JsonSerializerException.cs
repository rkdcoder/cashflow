using CashFlow.Shared.Exceptions.Core;
using Microsoft.AspNetCore.Http;

namespace CashFlow.Shared.Exceptions.Application
{
    /// <summary>
    /// Represents errors that occur during JSON serialization or deserialization operations.
    /// Provides more specific error information than standard JSON exceptions.
    /// </summary>
    public class JsonSerializerException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public JsonSerializerException(string message, Exception? inner = null)
            : base(message, StatusCodes.Status400BadRequest) { }
    }
}