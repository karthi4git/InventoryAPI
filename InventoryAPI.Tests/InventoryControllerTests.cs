using Moq;
using NUnit.Framework;
using InventoryAPI.Controllers;
using InventoryAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryAPI.Tests
{
    [TestFixture]
    public class InventoryControllerTests
    {
        private InventoryContext _dbContext;
        private InventoryController _controller;
        private Mock<ILogger<InventoryController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new InventoryContext(options);

            _mockLogger = new Mock<ILogger<InventoryController>>();
            _controller = new InventoryController(_dbContext, _mockLogger.Object);

            _dbContext.InventoryItems.AddRange(new List<InventoryItem>
            {
                new InventoryItem { ItemId = 1, Name = "Item1", Quantity = 10, Price = 100 },
                new InventoryItem { ItemId = 2, Name = "Item2", Quantity = 20, Price = 200 }
            });
            _dbContext.SaveChanges();
        }

        [Test]
        public async Task GetInventory_ReturnsOkResult_WithItems()
        {
            var result = await _controller.GetInventory();
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var items = okResult.Value as List<InventoryItem>;
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public async Task AddItem_ReturnsOkResult_WithNewItem()
        {
            var newItem = new InventoryItem { ItemId = 3, Name = "Item3", Quantity = 30, Price = 300 };

            var result = await _controller.AddItem(newItem);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(newItem, okResult.Value);
        }

        [Test]
        public async Task UpdateItem_ReturnsOkResult_WithUpdatedItem()
        {
            var updatedItem = new InventoryItem { Name = "UpdatedItem1", Quantity = 50, Price = 500 };

            var result = await _controller.UpdateItem(1, updatedItem);
            var okResult = result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var item = _dbContext.InventoryItems.FirstOrDefault(i => i.ItemId == 1);
            Assert.AreEqual("UpdatedItem1", item.Name);
            Assert.AreEqual(50, item.Quantity);
            Assert.AreEqual(500, item.Price);
        }

        [Test]
        public async Task DeleteItem_ReturnsOkResult_WhenItemExists()
        {
            var result = await _controller.DeleteItem(1);
            var okResult = result as OkResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNull(_dbContext.InventoryItems.FirstOrDefault(i => i.ItemId == 1));
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
