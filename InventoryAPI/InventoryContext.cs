using InventoryAPI.Model;
using Microsoft.EntityFrameworkCore;
namespace InventoryAPI
{
    public class InventoryContext:DbContext
    {
        public DbSet<InventoryItem> InventoryItems { get; set; }

        public InventoryContext(DbContextOptions<InventoryContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryItem>().HasKey(i => i.Id);
        }

    }
}
