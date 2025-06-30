namespace CashFlow.Entries.Domain.Entries.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Guid EntryTypeId { get; private set; }
        public EntryType EntryType { get; private set; } = null!;
        private Category() { }

        public Category(Guid id, string name, Guid entryTypeId)
        {
            Id = id;
            Name = name;
            EntryTypeId = entryTypeId;
        }
    }
}