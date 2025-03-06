using InventoryAPI.Controllers;
using InventoryAPI.Model;
using InventoryAPI.Repositories;
using InventoryAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

public class InventoryRepositoryTests
{
    private InventoryContext GetContext()
    {
        var options = new DbContextOptionsBuilder<InventoryContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new InventoryContext(options);
    }

    private ILogger<InventoryController> GetLogger()
    {
        return new NullLogger<InventoryController>();
    }

    [Fact]
    public async Task AddAsync_ShouldAddItemToDatabase()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());
        var item = new InventoryItem { Id = 1, ProductName = "Test", Quantity = 10 };

        // Act
        var result = await repository.AddAsync(item);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Id, result.Id);
        var dbItem = await context.InventoryItems.FindAsync(item.Id);
        Assert.Equal(item.ProductName, dbItem.ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingItem_ReturnsItem()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());
        var item = new InventoryItem { Id = 1, ProductName = "Test", Quantity = 10 };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingItem_ReturnsNull()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());
        context.InventoryItems.AddRange(
            new InventoryItem { Id = 1, ProductName = "Test1" },
            new InventoryItem { Id = 2, ProductName = "Test2" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ExistingItem_UpdatesItem()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());
        var existingItem = new InventoryItem { Id = 1, ProductName = "OldName", Quantity = 5 };
        context.InventoryItems.Add(existingItem);
        await context.SaveChangesAsync();

        var updatedItem = new InventoryItem { Id = 1, ProductName = "NewName", Quantity = 10 };

        // Act
        var result = await repository.UpdateAsync(updatedItem);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewName", result.ProductName);
        Assert.Equal(10, result.Quantity);
        var dbItem = await context.InventoryItems.FindAsync(1);
        Assert.Equal("NewName", dbItem.ProductName);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingItem_ReturnsNull()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());
        var item = new InventoryItem { Id = 1 };

        // Act
        var result = await repository.UpdateAsync(item);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingItem_RemovesItemAndReturnsTrue()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());
        var item = new InventoryItem { Id = 1 };
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.DeleteAsync(1);

        // Assert
        Assert.True(result);
        var dbItem = await context.InventoryItems.FindAsync(1);
        Assert.Null(dbItem);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingItem_ReturnsFalse()
    {
        // Arrange
        using var context = GetContext();
        var repository = new InventoryRepository(context, GetLogger());

        // Act
        var result = await repository.DeleteAsync(1);

        // Assert
        Assert.False(result);
    }
}