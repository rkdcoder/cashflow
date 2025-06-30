namespace CashFlow.Entries.Domain.Entries.Entities
{
    public class EntryType
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;

        public ICollection<Category> Categories { get; private set; } = new List<Category>();

        private EntryType() { }

        public EntryType(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}