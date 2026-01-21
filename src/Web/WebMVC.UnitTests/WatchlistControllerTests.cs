using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RTCodingExercise.Microservices.Controllers;
using RTCodingExercise.Microservices.Models;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace WebMVC.UnitTests;

/// <summary>
/// Unit tests for WatchlistController, verifying integration with Catalog API
/// according to the Regtransfers Code Exercise requirements
/// </summary>
public class WatchlistControllerTests
{
    private readonly Mock<ILogger<WatchlistController>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly WatchlistController _controller;

    public WatchlistControllerTests()
    {
        _mockLogger = new Mock<ILogger<WatchlistController>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5000")
        };

        _mockHttpClientFactory
            .Setup(f => f.CreateClient("CatalogApi"))
            .Returns(httpClient);

        _controller = new WatchlistController(
            _mockLogger.Object,
            _mockHttpClientFactory.Object,
            _mockConfiguration.Object);

        // Initialize TempData with a simple dictionary
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());
    }

    #region User Story - watchlist tests

    [Fact]
    public async Task Index_WithValidWatchlistItems_ReturnsViewWithItems()
    {
        // Arrange
        var watchlistItems = new List<WatchlistItem>
    {
        new WatchlistItem
        {
            Id = Guid.NewGuid(),
            WatchedPlate =  CreateTestPlates(1).First(),
            WatchPrice = 150.00m,
            UserId = Guid.NewGuid(),
            DateCreated = DateTime.Now,

        },
        new WatchlistItem
        {
            Id = Guid.NewGuid(),
            WatchedPlate =  CreateTestPlates(1).First(),
            WatchPrice = 150.00m,
            UserId = Guid.NewGuid(),
            DateCreated = DateTime.Now,
        }
    };

        SetupHttpResponse("/api/Watchlist", watchlistItems);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedItems = Assert.IsType<List<WatchlistItem>>(viewResult.Model);
        Assert.Equal(2, returnedItems.Count);
        Assert.Equal(watchlistItems[0].Id, returnedItems[0].Id);
        Assert.Equal(watchlistItems[1].WatchPrice, returnedItems[1].WatchPrice);
    }


    [Fact]
    public async Task Index_WitNoValidWatchlistItems_ReturnsViewWithNoItems()
    {
        // Arrange
        var emoptyWatchlistItems = new List<WatchlistItem>();

        SetupHttpResponse("/api/Watchlist", emoptyWatchlistItems);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var returnedItems = Assert.IsType<List<WatchlistItem>>(viewResult.Model);
        Assert.Empty(returnedItems);
    }
    [Fact]
    public async Task UpdateWatchPrice_WithValidIdAndPrice_SetsSuccessTempDataAndRedirects()
    {
        // Arrange
        var watchlistItemId = Guid.NewGuid();
        var newPrice = 175.50m;
        SetupHttpPostResponse($"/api/Watchlist/{watchlistItemId}/update-price", HttpStatusCode.OK);

        // Act
        var result = await _controller.UpdateWatchPrice(watchlistItemId, newPrice);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(WatchlistController.Index), redirectResult.ActionName);
        Assert.Equal("Watch price updated successfully!", _controller.TempData["Success"]);
    }

    [Fact]
    public async Task Remove_WithValidId_SetsSuccessTempDataAndRedirects()
    {
        // Arrange
        var watchlistItemId = Guid.NewGuid();
        SetupHttpPostResponse($"/api/Watchlist/{watchlistItemId}/remove", HttpStatusCode.OK);

        // Act
        var result = await _controller.Remove(watchlistItemId);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(WatchlistController.Index), redirectResult.ActionName);
        Assert.Equal("Item removed successfully!", _controller.TempData["Success"]);
    }

    #region Helper Methods

    private List<Plate> CreateTestPlates(int count)
    {
        var plates = new List<Plate>();
        for (int i = 0; i < count; i++)
        {
            plates.Add(new Plate
            {
                Id = Guid.NewGuid(),
                Registration = $"AB{i:D2} CDE",
                PurchasePrice = 100 + (i * 10),
                SalePrice = 120 + (i * 12),
                Status = PlateStatus.ForSale
            });
        }
        return plates;
    }

    private void SetupHttpResponse<T>(string requestUri, T responseData)
    {
        var jsonResponse = JsonSerializer.Serialize(responseData);
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.PathAndQuery.Contains(requestUri)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
    }

    private void SetupHttpPostResponse(string requestUri, HttpStatusCode statusCode)
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent("", Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri != null &&
                    req.RequestUri.PathAndQuery.Contains(requestUri)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
    }

    #endregion

}
    #endregion