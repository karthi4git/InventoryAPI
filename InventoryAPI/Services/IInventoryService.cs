using InventoryAPI.Model;

namespace InventoryAPI.Services
{
    public interface IInventoryService
    {
        Task<InventoryItem> CreateItemAsync(InventoryItem item);
        Task<IEnumerable<InventoryItem>> GetAllItemsAsync();
        Task<InventoryItem?> UpdateItemAsync(InventoryItem item);
        Task<bool> DeleteItemAsync(int id);
    }
}
