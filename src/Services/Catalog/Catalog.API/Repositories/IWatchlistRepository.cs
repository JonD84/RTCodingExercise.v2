namespace Catalog.API.Repositories;

public interface IWatchlistRepository
{
    Task<IEnumerable<WatchlistItem>> GetAllAsync();
    Task<WatchlistItem?> GetByIdAsync(Guid id);
    Task<WatchlistItem> AddAsync(WatchlistItem item);
    Task UpdatePriceAsync(WatchlistItem item);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}