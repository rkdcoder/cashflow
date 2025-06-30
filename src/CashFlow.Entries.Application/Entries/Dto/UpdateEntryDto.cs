namespace CashFlow.Entries.Application.Entries.Dto
{
    public class UpdateEntryDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}