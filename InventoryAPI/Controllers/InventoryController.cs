using InventoryAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : Controller
    {
        private readonly InventoryContext _context;

        public InventoryController(InventoryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {
            return Ok(await _context.InventoryItems.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, InventoryItem updatedItem)
        {
            var existingItem = await _context.InventoryItems.FindAsync(id);
            if (existingItem == null)
                return NotFound();

            existingItem.Name = updatedItem.Name;
            existingItem.Quantity = updatedItem.Quantity;
            existingItem.Price = updatedItem.Price;

            await _context.SaveChangesAsync();
            return Ok(existingItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
