namespace Catalog.API.Models;

public class WatchlistItem
{
    public Guid Id { get; set; }
    public Plate? WatchedPlate { get; set; }
    public decimal WatchPrice { get; set; }
    public decimal? LastKnownPrice { get; set; }
    public decimal? LastNotifiedPrice { get; set; }
    public Guid? UserId { get; set; }
    public DateTime DateCreated { get; set; }
}
