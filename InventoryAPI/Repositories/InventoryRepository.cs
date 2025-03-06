using InventoryAPI.Controllers;
using InventoryAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Repositories
{
    public class InventoryRepository:IInventoryRepository
    {

        private readonly InventoryContext _context;

        public InventoryRepository(InventoryContext context, ILogger<InventoryController> logger)
        {
            _context = context;
        }

        public async Task<InventoryItem> AddAsync(InventoryItem item)
        {
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InventoryItem?> GetByIdAsync(int id)
        {
            return await _context.InventoryItems.FindAsync(id);
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await _context.InventoryItems.ToListAsync();
        }

        public async Task<InventoryItem> UpdateAsync(InventoryItem item)
        {
            var existingItem = await _context.InventoryItems.FindAsync(item.Id);
            if (existingItem == null)
                return null;

            existingItem.ProductName = item.ProductName;
            //existingItem.Description = item.des;
            existingItem.Quantity = item.Quantity;
            existingItem.ShipmentDate = item.ShipmentDate;
            //existingItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return false;

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}