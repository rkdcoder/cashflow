namespace CashFlow.Entries.Domain.Entries.Entities
{
    public class Entry
    {
        public Guid Id { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public Guid CategoryId { get; private set; }
        public Guid EntryTypeId { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid? ModifiedBy { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        public Category? Category { get; private set; }
        public EntryType? EntryType { get; private set; }

        private Entry() { }

        public Entry(
            decimal amount,
            string description,
            Guid entryTypeId,
            Guid categoryId,
            Guid createdBy)
        {
            Id = Guid.NewGuid();
            Amount = amount;
            Description = description;
            EntryTypeId = entryTypeId;
            CategoryId = categoryId;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(decimal amount, string description, Guid entryTypeId, Guid categoryId, Guid modifiedBy)
        {
            Amount = amount;
            Description = description;
            EntryTypeId = entryTypeId;
            CategoryId = categoryId;
            ModifiedBy = modifiedBy;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}