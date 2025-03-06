using Xunit;
using Moq;
using InventoryAPI.Model;
using InventoryAPI.Repositories;
using InventoryAPI.Services;
using Assert = Xunit.Assert;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _mockRepo;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _mockRepo = new Mock<IInventoryRepository>();
        _service = new InventoryService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetItemByIdAsync_ValidId_ReturnsItem()
    {
        // Arrange
        var testItem = new InventoryItem { Id = 1, ProductName = "Laptop", Quantity = 10,ShipmentDate=Convert.ToDateTime("2025-05-03") };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(testItem);

        // Act
        var result = await _service.GetItemByIdAsync(1);

        // Assert
        Assert.Equal(testItem, result);
    }

    [Fact]
    public async Task GetItemByIdAsync_InvalidId_ThrowsException()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((InventoryItem)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetItemByIdAsync(999));
    }

    [Fact]
    public async Task AddItemAsync_NegativeQuantity_ThrowsException()
    {
        // Arrange
        var invalidItem = new InventoryItem { Quantity = -5 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateItemAsync(invalidItem));
    }

    [Fact]
    public async Task UpdateItemAsync_ValidItem_UpdatesSuccessfully()
    {
        // Arrange
        var existingItem = new InventoryItem { Id = 1, ProductName = "Old Item", Quantity = 5 };
        var updatedItem = new InventoryItem { Id = 1, ProductName = "Updated Item", Quantity = 10 };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);

        // Act
        await _service.UpdateItemAsync(updatedItem);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<InventoryItem>()), Times.Once);
        
    }
}
