using CashFlow.Entries.Domain.Entries.Entities;

namespace CashFlow.Entries.Domain.Entries.Repositories
{
    public interface IEntryRepository
    {
        Task AddAsync(Entry entry);
        Task UpdateAsync(Entry entry);
        Task DeleteAsync(Entry entry);
        Task<List<Guid>> GetIdsByDateRangeAsync(DateTime start, DateTime end);
        Task<Entry?> GetByIdAsync(Guid id);
    }
}