using System.Text.Json.Nodes;
using Catalog.API.Models;


namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;
    private readonly ILogger<WatchlistController> _logger;

    public WatchlistController(IWatchlistService watchlistService, ILogger<WatchlistController> logger)
    {
        _watchlistService = watchlistService;
        _logger = logger;
    }

    /// <summary>
    /// Get list of watchlist items
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Plate>>> GetWatchlist()
    {
        var result = await _watchlistService.GetItemsAsync();
        return Ok(result);
    }

     /// <summary>
    /// Remove a plate from watchlist
    /// </summary>
    [HttpPost("/api/Watchlist")]
    [ProducesResponseType(typeof(Models.WatchlistItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddToWatchlist([FromBody]JsonObject request)
    {
        var addToWatchlistRequest = System.Text.Json.JsonSerializer.Deserialize<AddToWatchlistRequest>(request);

        if(addToWatchlistRequest == null || addToWatchlistRequest.PlateId == Guid.Empty)
        {
            return BadRequest(new { error = "Plate Id is not vaild." });
        }

        try
        {
            await _watchlistService.CreateItemAsync(addToWatchlistRequest.PlateId, addToWatchlistRequest.CustomerId, null);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// remove item from watchlist
    /// </summary>
    [HttpPost("{id}/remove")]
    [ProducesResponseType(typeof(Domain.WatchlistItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Plate>> Remove(Guid id)
    {
        try
        {
            await _watchlistService.DeletePlateAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update the watch price for a watchlist item
    /// </summary>
    [HttpPost("{id}/update-price")]
    [ProducesResponseType(typeof(Models.WatchlistItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateWatchPrice(Guid id, decimal price)
    {
    
        try
        {
            await _watchlistService.UpdateItemAsync(id, price);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}