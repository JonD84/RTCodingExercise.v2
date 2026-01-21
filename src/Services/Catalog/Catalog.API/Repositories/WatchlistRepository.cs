using Catalog.API.Data;
using Catalog.API.Services;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WatchlistRepository> _logger;
    public WatchlistRepository(ApplicationDbContext context, ILogger<WatchlistRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<WatchlistItem>> GetAllAsync()
    {
        return await _context.WatchListItems.ToListAsync();
    }

    public async Task<WatchlistItem?> GetByIdAsync(Guid id)
    {
        return await _context.WatchListItems.FindAsync(id);
    }

     public async Task<WatchlistItem> AddAsync(WatchlistItem item)
    {
        await _context.WatchListItems.AddAsync(item);
        return item;
    }

    public async Task UpdatePriceAsync(WatchlistItem item)
    {
        _context.WatchListItems.Update(item);
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        if (item != null)
        {
            _context.WatchListItems.Remove(item);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}