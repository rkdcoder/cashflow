namespace CashFlow.Consolidations.Application.DailyEntries.Dto
{
    public record ConsolidationEntryDto
    {
        public Guid Id { get; init; }
        public decimal Amount { get; init; }
        public string Description { get; init; } = string.Empty;
        public Guid CategoryId { get; init; }
        public Guid EntryTypeId { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}