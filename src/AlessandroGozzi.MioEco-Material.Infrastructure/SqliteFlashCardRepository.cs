using System;
using System.Collections.Generic;
using System.Text;
using AlessandroGozzi.MioEco_Material.Domain;
using AlessandroGozzi.MioEco_Material.Domain.Interfaces;
using SQLite;

namespace AlessandroGozzi.MioEco_Material.Infrastructure
{
    public class SqliteFlashCardRepository : IFlashCardRepository
    {
        private readonly AppDbContext _context;

        public SqliteFlashCardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlashCard>> GetAllAsync()
        {
            var db = await _context.GetConnectionAsync();
            return await db.Table<FlashCard>()
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.UsageCount)
                .ToListAsync();
        }

        public async Task<FlashCard?> GetByIdAsync(Guid id)
        {
            var db = await _context.GetConnectionAsync();
            return await db.Table<FlashCard>()
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<List<FlashCard>> GetByCategoryAsync(Category category)
        {
            var db = await _context.GetConnectionAsync();
            return await db.Table<FlashCard>()
                .Where(c => !c.IsDeleted && c.Category == category)
                .OrderByDescending(c => c.UsageCount)
                .ToListAsync();
        }

        public async Task AddAsync(FlashCard flashCard)
        {
            var db = await _context.GetConnectionAsync();
            await db.InsertAsync(flashCard);
        }

        public async Task UpdateAsync(FlashCard flashCard)
        {
            var db = await _context.GetConnectionAsync();
            await db.UpdateAsync(flashCard);
        }

        public async Task DeleteAsync(Guid id)
        {
            var db = await _context.GetConnectionAsync();
            var card = await GetByIdAsync(id);
            if (card != null)
            {
                card.IsDeleted = true;
                await db.UpdateAsync(card);
            }
        }

        public async Task IncrementUsageAsync(Guid id)
        {
            var db = await _context.GetConnectionAsync();
            var card = await GetByIdAsync(id);
            if (card != null)
            {
                card.IncrementUsageCount();
                await db.UpdateAsync(card);
            }
        }
    }
}
