namespace CashFlow.Entries.Application.Categories.Dto
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid EntryTypeId { get; set; }
        public string EntryTypeName { get; set; } = string.Empty;
    }
}