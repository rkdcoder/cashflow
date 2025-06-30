namespace CashFlow.Entries.Application.Entries.IntegrationEvent
{
    public class EntryChangedIntegrationEvent
    {
        public Guid EntryId { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public Guid UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal Amount { get; set; }
        public Guid CategoryId { get; set; }
        public Guid EntryTypeId { get; set; }
    }
}
