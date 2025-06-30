using CashFlow.Entries.Domain.Entries.Entities;

namespace CashFlow.Entries.Domain.Categories.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetAllWithEntryTypeAsync();
        Task<Category?> GetByNameAsync(string name);
        Task<Category?> GetByIdAsync(Guid id);
        Task AddAsync(Category category);
    }
}