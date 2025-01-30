using InventoryAPI.Controllers;
using InventoryAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryAPI.Tests
{
    [TestFixture]
    public class InventoryControllerTests
    {
        private DbContextOptions<InventoryContext> _options;

        [SetUp]
        public void Setup()
        {
            // Initialize a fresh in-memory database for each test
            _options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Test]
        public async Task GetInventory_ReturnsAllItems()
        {
            // Arrange: Add test data
            using (var context = new InventoryContext(_options))
            {
                context.InventoryItems.AddRange(
                    new InventoryItem { ItemId = 1, Name = "Item1" },
                    new InventoryItem { ItemId = 2, Name = "Item2" }
                );
                await context.SaveChangesAsync();
            }

            // Act: Call the API
            using (var context = new InventoryContext(_options))
            {
                var controller = new InventoryController(context);
                var result = await controller.GetInventory();

                // Assert: Check the response
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var okResult = result as OkObjectResult;
                var items = okResult.Value as List<InventoryItem>;
                Assert.That(items, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public async Task AddItem_AddsItemToDatabase()
        {
            // Arrange: Create a new item
            var newItem = new InventoryItem { ItemId = 1, Name = "New Item", Quantity = 10, Price = 19.99m };

            // Act: Add the item
            using (var context = new InventoryContext(_options))
            {
                var controller = new InventoryController(context);
                var result = await controller.AddItem(newItem);

                // Assert: Check the response and database
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var okResult = result as OkObjectResult;
                var returnedItem = okResult.Value as InventoryItem;
                Assert.That(returnedItem.ItemId, Is.EqualTo(1));

                // Verify the item is saved
                var itemInDb = await context.InventoryItems.FindAsync(1);
                Assert.That(itemInDb, Is.Not.Null);
                Assert.That(itemInDb.Name, Is.EqualTo("New Item"));
            }
        }

        [Test]
        public async Task UpdateItem_UpdatesExistingItem()
        {
            // Arrange: Add an item to update
            using (var context = new InventoryContext(_options))
            {
                context.InventoryItems.Add(new InventoryItem { ItemId = 1, Name = "Old Name" });
                await context.SaveChangesAsync();
            }

            var updatedItem = new InventoryItem { ItemId = 1, Name = "New Name", Quantity = 15, Price = 20m };

            // Act: Update the item
            using (var context = new InventoryContext(_options))
            {
                var controller = new InventoryController(context);
                var result = await controller.UpdateItem(1, updatedItem);

                // Assert: Check response and database
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var itemInDb = await context.InventoryItems.FindAsync(1);
                Assert.That(itemInDb.Name, Is.EqualTo("New Name"));
            }
        }

        [Test]
        public async Task UpdateItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange: No items in the database
            var updatedItem = new InventoryItem { ItemId = 999, Name = "Non-existent" };

            // Act: Attempt to update
            using (var context = new InventoryContext(_options))
            {
                var controller = new InventoryController(context);
                var result = await controller.UpdateItem(999, updatedItem);

                // Assert: Check for NotFound
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            }
        }

        [Test]
        public async Task DeleteItem_RemovesItemFromDatabase()
        {
            // Arrange: Add an item to delete
            using (var context = new InventoryContext(_options))
            {
                context.InventoryItems.Add(new InventoryItem { ItemId = 1, Name = "Delete Me" });
                await context.SaveChangesAsync();
            }

            // Act: Delete the item
            using (var context = new InventoryContext(_options))
            {
                var controller = new InventoryController(context);
                var result = await controller.DeleteItem(1);

                // Assert: Check response and database
                Assert.That(result, Is.InstanceOf<OkResult>());
                var itemInDb = await context.InventoryItems.FindAsync(1);
                Assert.That(itemInDb, Is.Null);
            }
        }

        [Test]
        public async Task DeleteItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Act: Attempt to delete non-existent item
            using (var context = new InventoryContext(_options))
            {
                var controller = new InventoryController(context);
                var result = await controller.DeleteItem(999);

                // Assert: Check for NotFound
                Assert.That(result, Is.InstanceOf<NotFoundResult>());
            }
        }
    }
}