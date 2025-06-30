using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Entries.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Entries.Infrastructure.Repositories
{
    public class EntryRepository : IEntryRepository
    {
        private readonly EntriesDbContext _context;

        public EntryRepository(EntriesDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Entry entry)
        {
            await _context.Entries.AddAsync(entry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Entry entry)
        {
            _context.Entries.Update(entry);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Entry entry)
        {
            _context.Entries.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetIdsByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Entries
                .Where(e => e.CreatedAt >= start && e.CreatedAt <= end)
                .Select(e => e.Id)
                .ToListAsync();
        }

        public async Task<Entry?> GetByIdAsync(Guid id)
        {
            return await _context.Entries
                .Include(e => e.Category)
                .Include(e => e.EntryType)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}