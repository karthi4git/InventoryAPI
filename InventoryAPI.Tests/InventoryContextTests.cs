using InventoryAPI;
using InventoryAPI.Model;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace InventoryAPI.Tests
{
    [TestFixture]
    public class InventoryContextTests
    {
        private DbContextOptions<InventoryContext> _options;

        [SetUp]
        public void Setup()
        {
            // Use a fresh in-memory database for each test
            _options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for isolation
                .Options;
        }

        [Test]
        public void AddInventoryItem_ShouldPersistToDatabase()
        {
            // Arrange
            var item = new InventoryItem { ItemId = 1, Name = "Test Item" };

            // Act
            using (var context = new InventoryContext(_options))
            {
                context.InventoryItems.Add(item);
                context.SaveChanges();
            }

            // Assert
            using (var context = new InventoryContext(_options))
            {
                var savedItem = context.InventoryItems.Find(1);
                Assert.That(savedItem, Is.Not.Null);
                Assert.That(savedItem.Name, Is.EqualTo("Test Item"));
            }
        }

        [Test]
        public void OnModelCreating_ShouldSetItemIdAsPrimaryKey()
        {
            // Arrange & Act
            using (var context = new InventoryContext(_options))
            {
                var entityType = context.Model.FindEntityType(typeof(InventoryItem));
                var primaryKey = entityType.FindPrimaryKey();

                // Assert
                Assert.That(primaryKey.Properties, Has.Exactly(1).Items);
                Assert.That(primaryKey.Properties[0].Name, Is.EqualTo("ItemId"));
            }
        }
    }
}