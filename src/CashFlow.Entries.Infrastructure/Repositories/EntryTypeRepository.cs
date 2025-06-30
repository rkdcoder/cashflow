using CashFlow.Entries.Domain.Entries.Entities;
using CashFlow.Entries.Domain.Entries.Repositories;
using CashFlow.Entries.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace CashFlow.Entries.Infrastructure.Repositories
{
    public class EntryTypeRepository : IEntryTypeRepository
    {
        private readonly EntriesDbContext _context;
        public EntryTypeRepository(EntriesDbContext context)
        {
            _context = context;
        }

        public async Task<EntryType?> GetByIdAsync(Guid id)
        {
            return await _context.EntryTypes.FindAsync(id);
        }

        public async Task<List<EntryType>> GetAllAsync()
        {
            return await _context.EntryTypes.ToListAsync();
        }
    }
}