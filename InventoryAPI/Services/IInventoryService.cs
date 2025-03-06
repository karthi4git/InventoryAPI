using InventoryAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Services
{
    public interface IInventoryService
    {
        Task<InventoryItem> CreateItemAsync(InventoryItem item);
        Task<ActionResult<InventoryItem>> GetItemByIdAsync(int id);
        Task<IEnumerable<InventoryItem>> GetAllItemsAsync();
        Task<InventoryItem?> UpdateItemAsync(InventoryItem item);
        Task<bool> DeleteItemAsync(int id);
    }
}
