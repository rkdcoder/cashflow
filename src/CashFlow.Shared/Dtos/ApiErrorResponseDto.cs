using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashFlow.Shared.Dtos
{
    /// <summary>
    /// Standardized error response for API exceptions.
    /// </summary>
    public class ApiErrorResponseDto
    {
        /// <summary>
        /// Fixed false return status to standardize the command returns.
        /// </summary>
        public bool Success => false;

        /// <summary>
        /// Mensagem amigável de erro.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// (Optional) Detailed technical error information (only in debug).
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// (Optional) Request path where the error occurred.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// (Optional) Unique request trace identifier.
        /// </summary>
        public string? TraceId { get; set; }
    }
}