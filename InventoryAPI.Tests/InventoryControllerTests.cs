using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryAPI.Controllers;
using InventoryAPI.Model;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

public class InventoryControllerTests
{
    private readonly Mock<IInventoryService> _inventoryServiceMock;
    private readonly Mock<ILogger<InventoryController>> _loggerMock;
    private readonly InventoryController _controller;

    public InventoryControllerTests()
    {
        _inventoryServiceMock = new Mock<IInventoryService>();
        _loggerMock = new Mock<ILogger<InventoryController>>();
        _controller = new InventoryController(_inventoryServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetInventory_ReturnsOkWithItems()
    {
        // Arrange
        var expectedItems = new List<InventoryItem> { new InventoryItem { Id = 1, ProductName = "Test Item" } };
        _inventoryServiceMock.Setup(s => s.GetAllItemsAsync()).ReturnsAsync(expectedItems);

        // Act
        var result = await _controller.GetInventory();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedItems, okResult.Value);

        // Verify logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching all inventory items.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetInventory_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _inventoryServiceMock.Setup(s => s.GetAllItemsAsync()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetInventory();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);

        // Verify error logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while retrieving inventory items.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetItem_ExistingId_ReturnsItem()
    {
        // Arrange
        var testItem = new InventoryItem { Id = 1, ProductName = "Test Item" };
        _inventoryServiceMock.Setup(s => s.GetItemByIdAsync(1)).ReturnsAsync(testItem);

        // Act
        var result = await _controller.GetItem(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(testItem, okResult.Value);
    }

    [Fact]
    public async Task GetItem_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _inventoryServiceMock.Setup(s => s.GetItemByIdAsync(It.IsAny<int>())).ReturnsAsync((InventoryItem)null);

        // Act
        var result = await _controller.GetItem(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddItem_ValidItem_ReturnsCreatedAtAction()
    {
        // Arrange
        var testItem = new InventoryItem { Id = 1, ProductName = "New Item" };
        _inventoryServiceMock.Setup(s => s.CreateItemAsync(testItem)).Returns(Task.CompletedTask.ToString);

        // Act
        var result = await _controller.AddItem(testItem);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(InventoryController.GetItem), createdAtResult.ActionName);
        Assert.Equal(testItem.Id, createdAtResult.RouteValues["id"]);
        Assert.Equal(testItem, createdAtResult.Value);

        // Verify logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Adding new item: {testItem.ProductName}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task AddItem_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _inventoryServiceMock.Setup(s => s.CreateItemAsync(It.IsAny<InventoryItem>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AddItem(new InventoryItem());

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);

        // Verify error logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while adding an item.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateItem_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var testItem = new InventoryItem { Id = 2 };

        // Act
        var result = await _controller.UpdateItem(1, testItem);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ID mismatch", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateItem_ValidUpdate_ReturnsNoContent()
    {
        // Arrange
        var testItem = new InventoryItem { Id = 1 };
        _inventoryServiceMock.Setup(s => s.UpdateItemAsync(testItem)).Returns(Task.CompletedTask.ToString);

        // Act
        var result = await _controller.UpdateItem(1, testItem);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _inventoryServiceMock.Verify(s => s.UpdateItemAsync(testItem), Times.Once);
    }

    [Fact]
    public async Task UpdateItem_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var testItem = new InventoryItem { Id = 1 };
        _inventoryServiceMock.Setup(s => s.UpdateItemAsync(testItem)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.UpdateItem(1, testItem);

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);

        // Verify error logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Error occurred while updating item with ID: {testItem.Id}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteItem_ExistingId_ReturnsNoContent()
    {
        // Arrange
        const int testId = 1;
        _inventoryServiceMock.Setup(s => s.DeleteItemAsync(testId)).Returns(Task.CompletedTask.ToString);

        // Act
        var result = await _controller.DeleteItem(testId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _inventoryServiceMock.Verify(s => s.DeleteItemAsync(testId), Times.Once);
    }

    [Fact]
    public async Task DeleteItem_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        const int testId = 1;
        _inventoryServiceMock.Setup(s => s.DeleteItemAsync(testId)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.DeleteItem(testId);

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);

        // Verify error logging
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Error occurred while deleting item with ID: {testId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}