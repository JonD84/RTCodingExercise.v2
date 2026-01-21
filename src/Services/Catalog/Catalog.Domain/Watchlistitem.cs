namespace Catalog.Domain;

public class WatchlistItem
{
    public Guid Id { get; set; }
    public Guid PlateId { get; set; }
    public decimal WatchPrice { get; set; }
    public decimal? LastKnownPrice { get; set; }
    public decimal? LastNotifiedPrice { get; set; }
    public Guid? UserId { get; set; }
    public DateTime DateCreated { get; set; }

    // To track if notification has been sent for this watchlist item, this has not been implemented yet
    public bool NotificaionSent { get; set; } = false;
}
