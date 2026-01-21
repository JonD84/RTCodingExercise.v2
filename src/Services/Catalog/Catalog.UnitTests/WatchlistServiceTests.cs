using Catalog.API.Models;
using Catalog.API.Repositories;
using Catalog.API.Services;
using Catalog.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IntegrationEvents;

namespace Catalog.UnitTests;

/// <summary>
/// Unit tests for PlateService, verifying business logic and service layer operations
/// according to the Regtransfers Code Exercise requirements
/// </summary>
public class WatchlistServiceTests
{
    private readonly Mock<IWatchlistRepository> _mockWatchlistRepository;
    private readonly Mock<IPlateRepository> _mockPlateRepository;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly Mock<ILogger<WatchlistService>> _mockLogger;
    private readonly WatchlistService _service;

    public WatchlistServiceTests()
    {
        _mockWatchlistRepository = new Mock<IWatchlistRepository>();
        _mockPlateRepository = new Mock<IPlateRepository>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _mockLogger = new Mock<ILogger<WatchlistService>>();
        _service = new WatchlistService(_mockWatchlistRepository.Object, _mockPlateRepository.Object, _mockPublishEndpoint.Object, _mockLogger.Object);
    }

    #region Watchlist service tests
    [Fact]
    public async Task CreateItemAsync_WithValidPlateIdAndPrice_CreatesAndReturnsItem()
    {
        // Arrange
        var plateId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var watchPrice = 150.00m;

        // Act
        var result = await _service.CreateItemAsync(plateId, customerId, watchPrice);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(plateId, result.PlateId);
        Assert.Equal(watchPrice, result.WatchPrice);
        Assert.Equal(customerId, result.UserId);
        _mockWatchlistRepository.Verify(r => r.AddAsync(It.IsAny<Domain.WatchlistItem>()), Times.Once);
        _mockWatchlistRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetItemsAsync_WithValidWatchlistItems_ReturnsItemsWithPlateData()
    {
        // Arrange
        var plateId1 = Guid.NewGuid();
        var plateId2 = Guid.NewGuid();
        var itemId1 = Guid.NewGuid();
        var itemId2 = Guid.NewGuid();

        var domainItems = new List<Domain.WatchlistItem>
        {
            new Domain.WatchlistItem { Id = itemId1, PlateId = plateId1, WatchPrice = 150.00m, UserId = Guid.NewGuid() },
            new Domain.WatchlistItem { Id = itemId2, PlateId = plateId2, WatchPrice = 200.00m, UserId = Guid.NewGuid() }
        };

        var plates = new List<Plate>
        {
            new Plate { Id = plateId1, Registration = "ABC 123", PurchasePrice = 160.00m },
            new Plate { Id = plateId2, Registration = "XYZ 789", PurchasePrice = 195.00m }
        };

        _mockWatchlistRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(domainItems);
        _mockPlateRepository.Setup(r => r.GetByIdAsync(plateId1)).ReturnsAsync(plates[0]);
        _mockPlateRepository.Setup(r => r.GetByIdAsync(plateId2)).ReturnsAsync(plates[1]);

        // Act
        var result = await _service.GetItemsAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal(itemId1, resultList[0].Id);
        Assert.NotNull(resultList[0].WatchedPlate);
        Assert.Equal(plateId1, resultList[0].WatchedPlate!.Id);
        Assert.Equal(150.00m, resultList[0].WatchPrice);
        Assert.Equal(itemId2, resultList[1].Id);
        Assert.NotNull(resultList[1].WatchedPlate);
        Assert.Equal(plateId2, resultList[1].WatchedPlate!.Id);
        _mockWatchlistRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockPlateRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateItemAsync_WithValidWatchlistItem_UpdatesWatchPrice()
    {
        // Arrange
        var itemId1 = Guid.NewGuid();
        var plateId1 = Guid.NewGuid();
        var userId1 = Guid.NewGuid();
        var newPrice = 175.00m;

        var existingItem = new Domain.WatchlistItem
        {
            Id = itemId1,
            PlateId = plateId1,
            WatchPrice = 150.00m,
            UserId = userId1
        };

        _mockWatchlistRepository.Setup(r => r.GetByIdAsync(itemId1)).ReturnsAsync(existingItem);

        // Act
        await _service.UpdateItemAsync(itemId1, newPrice);

        // Assert
        _mockWatchlistRepository.Verify(r => r.GetByIdAsync(itemId1), Times.Once);
        _mockWatchlistRepository.Verify(r => r.UpdatePriceAsync(It.Is<Domain.WatchlistItem>(item => 
            item.Id == itemId1 && item.WatchPrice == newPrice)), Times.Once);
        _mockWatchlistRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    #endregion
}
