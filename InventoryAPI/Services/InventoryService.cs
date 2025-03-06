using InventoryAPI.Model;
using InventoryAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Services
{
    public class InventoryService:IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<InventoryItem> CreateItemAsync(InventoryItem item)
        {
            // Business logic/validation
            if (item.Quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            return await _repository.AddAsync(item);
        }
        public async Task<ActionResult<InventoryItem>> GetItemByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<InventoryItem>> GetAllItemsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<InventoryItem?> UpdateItemAsync(InventoryItem item)
        {
            var existingItem = await _repository.GetByIdAsync(item.Id);
            if (existingItem == null)
                return null;

            // Additional business logic
            if (item.Quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            return await _repository.UpdateAsync(item);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
   
