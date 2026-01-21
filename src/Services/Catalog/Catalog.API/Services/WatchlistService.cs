using Catalog.API.Models;
using Catalog.API.Repositories;
using IntegrationEvents;
using MassTransit;


namespace Catalog.API.Services;

public class WatchlistService : IWatchlistService
{
    private readonly IWatchlistRepository _WatchlistRepository;
    private readonly IPlateRepository _PlateRepository;

     private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<WatchlistService> _logger;

    public WatchlistService(IWatchlistRepository watchlistRepository, IPlateRepository plateRepository, IPublishEndpoint publishEndpoint,ILogger<WatchlistService> logger)
    {
        _WatchlistRepository = watchlistRepository;
        _PlateRepository = plateRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<IEnumerable<Models.WatchlistItem>> GetItemsAsync()
    {
        var result = await _WatchlistRepository.GetAllAsync();
        var watchllist = new List<Models.WatchlistItem>();

        foreach (var item in result)
        {
            var plate = await _PlateRepository.GetByIdAsync(item.PlateId);

            if (plate != null)
            {
                watchllist.Add(new Models.WatchlistItem {
                    Id = item.Id,
                    WatchPrice = item.WatchPrice,
                    WatchedPlate= plate,
                    LastKnownPrice = item.LastKnownPrice,
                    LastNotifiedPrice = item.LastNotifiedPrice,
                    UserId = item.UserId, 
                });
            }
            else
            {
                throw new KeyNotFoundException($"Plate ID {item.PlateId} could not be found");
            }
        }
        
        return watchllist.AsEnumerable();
    }

    public async Task<Domain.WatchlistItem> CreateItemAsync(Guid plateId, Guid? customerId, decimal? watchPrice)
    {
        if(plateId == Guid.Empty)
        {
            throw new KeyNotFoundException($"Plate ID {plateId} is null or empty");
        }

        var item = new Domain.WatchlistItem
        {
            Id = Guid.NewGuid(),
            PlateId = plateId,
            WatchPrice = watchPrice ?? 0m,
            UserId = customerId,
            DateCreated = DateTime.UtcNow
        };

        await _WatchlistRepository.AddAsync(item);
        await _WatchlistRepository.SaveChangesAsync();
        return item;
    }
    public async Task DeletePlateAsync(Guid id)
    {
        await _WatchlistRepository.DeleteAsync(id);
        await _WatchlistRepository.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(Guid itemId, decimal newWatchPrice)
    {
        var watchlistItem = await _WatchlistRepository.GetByIdAsync(itemId).ConfigureAwait(false);

        if (watchlistItem == null)
        {
            throw new KeyNotFoundException($"Watchlist Item with ID {itemId} not found");
        }

        watchlistItem!.WatchPrice = newWatchPrice;

        await _WatchlistRepository.UpdatePriceAsync(watchlistItem);
        await _WatchlistRepository.SaveChangesAsync();
    }
}