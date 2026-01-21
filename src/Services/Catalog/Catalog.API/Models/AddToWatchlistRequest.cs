namespace Catalog.API.Models;

public class AddToWatchlistRequest
{
    public Guid PlateId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal? WatchPrice { get; set; }

}
