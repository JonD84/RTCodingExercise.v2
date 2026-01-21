using Catalog.API.Models;

namespace Catalog.API.Services;

public interface IWatchlistService 
{
    public Task<IEnumerable<Models.WatchlistItem>> GetItemsAsync();
    Task<Domain.WatchlistItem> CreateItemAsync(Guid plateId,Guid? userId, decimal? watchPrice);
    Task DeletePlateAsync(Guid id);
    Task UpdateItemAsync(Guid itemId, decimal newWatchPrice);
    
}