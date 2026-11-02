using System;
using System.Collections.Generic;
using System.Text;

namespace AlessandroGozzi.MioEco_Material.Domain.Interfaces
{
    public interface IFlashCardRepository
    {
        Task<List<FlashCard>> GetAllAsync();
        Task<FlashCard?> GetByIdAsync(Guid id);
        Task<List<FlashCard>> GetByCategoryAsync(Category category);
        Task AddAsync(FlashCard flashCard);
        Task UpdateAsync(FlashCard flashCard);
        Task DeleteAsync(Guid id); // Soft delete (IsDeleted = true)
        Task IncrementUsageAsync(Guid id);
    }
}
