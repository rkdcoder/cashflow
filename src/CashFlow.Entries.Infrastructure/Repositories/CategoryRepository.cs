using CashFlow.Entries.Domain.Categories.Repositories;
using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Entries.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EntriesDbContext _context;

        public CategoryRepository(EntriesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<List<Category>> GetAllWithEntryTypeAsync()
        {
            return await _context.Categories
                .Include(c => c.EntryType)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => EF.Functions.ILike(c.Name, name));
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
    }
}