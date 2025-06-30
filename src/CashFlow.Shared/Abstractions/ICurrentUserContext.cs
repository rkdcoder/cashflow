namespace CashFlow.Shared.Abstractions
{
    /// <summary>
    /// Provides access to the current authenticated user's context.
    /// </summary>
    public interface ICurrentUserContext
    {
        /// <summary>
        /// User unique identifier (subject/NameIdentifier).
        /// </summary>
        string? Subject { get; }

        /// <summary>
        /// User email.
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// User name.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// User roles.
        /// </summary>
        IReadOnlyList<string> Roles { get; }
    }
}