namespace CashFlow.Shared.Dtos
{
    /// <summary>
    /// Represents the result of a validation operation.
    /// </summary>
    public class ValidationResultDto
    {
        /// <summary>
        /// Indicates whether the validation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the reason for validation failure, or null if succeeded.
        /// </summary>
        public string? Message { get; set; }

        public static ValidationResultDto SuccessResult() =>
            new ValidationResultDto { Success = true };

        public static ValidationResultDto FailureResult(string message) =>
            new ValidationResultDto { Success = false, Message = message };
    }
}