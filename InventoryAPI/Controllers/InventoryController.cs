using InventoryAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryContext _context;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(InventoryContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            try
            {
                _logger.LogInformation("Fetching all inventory items.");
                var items = await _context.InventoryItems.AsNoTracking().ToListAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving inventory items.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(InventoryItem item)
        {
            try
            {
                _logger.LogInformation($"Adding new item: {item.Name}");
                _context.InventoryItems.Add(item);
                await _context.SaveChangesAsync();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding an item.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, InventoryItem updatedItem)
        {
            try
            {
                _logger.LogInformation($"Updating item with ID: {id}");
                var existingItem = await _context.InventoryItems.FindAsync(id);
                if (existingItem == null)
                    return NotFound();

                existingItem.Name = updatedItem.Name;
                existingItem.Quantity = updatedItem.Quantity;
                existingItem.Price = updatedItem.Price;

                await _context.SaveChangesAsync();
                return Ok(existingItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating item with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting item with ID: {id}");
                var item = await _context.InventoryItems.FindAsync(id);
                if (item == null)
                    return NotFound();

                _context.InventoryItems.Remove(item);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting item with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
