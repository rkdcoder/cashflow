using CashFlow.Entries.Domain.Entries.Entities;

namespace CashFlow.Entries.Domain.Entries.Repositories
{
    public interface IEntryTypeRepository
    {
        Task<EntryType?> GetByIdAsync(Guid id);
        Task<List<EntryType>> GetAllAsync();
    }
}