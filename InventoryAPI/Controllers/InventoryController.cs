using InventoryAPI.Model;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            try
            {
                _logger.LogInformation("Fetching all inventory items.");
                return Ok(await _inventoryService.GetAllItemsAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving inventory items.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetItem(int id)
        {
            var item = await _inventoryService.GetItemByIdAsync(id);
            return item != null ? Ok(item) : NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(InventoryItem item)
        {
            try
            {
                _logger.LogInformation($"Adding new item: {item.ProductName}");
                await _inventoryService.CreateItemAsync(item);
                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
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
                if (id != updatedItem.Id) return BadRequest("ID mismatch");
                await _inventoryService.UpdateItemAsync(updatedItem);
                return NoContent();
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
                await _inventoryService.DeleteItemAsync(id);
                return NoContent();               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting item with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
