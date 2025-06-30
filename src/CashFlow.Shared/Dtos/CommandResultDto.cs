namespace CashFlow.Shared.Dtos
{
    /// <summary>
    /// Standard command result for CQRS commands (Create, Update, Delete).
    /// </summary>
    /// <typeparam name="T">Type of the optional payload data.</typeparam>
    public class CommandResultDto<T>
    {
        /// <summary>
        /// Indicates whether the operation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Optional user-friendly message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional result data.
        /// </summary>
        public T? Data { get; set; }

        public CommandResultDto(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static CommandResultDto<T> SuccessResult(string message, T? data = default) =>
            new CommandResultDto<T>(true, message, data);

        public static CommandResultDto<T> FailureResult(string message, T? data = default) =>
            new CommandResultDto<T>(false, message, data);
    }
}