using InventoryAPI.Model;

namespace InventoryAPI.Repositories
{
    public interface IInventoryRepository
    {
        Task<InventoryItem> AddAsync(InventoryItem item);
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<InventoryItem?> GetByIdAsync(int id);
        Task<InventoryItem> UpdateAsync(InventoryItem item);
        Task<bool> DeleteAsync(int id);
    }
}
