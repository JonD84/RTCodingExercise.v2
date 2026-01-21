namespace IntegrationEvents;

public class WatchlistPriceChangeIntegrationEvent : IntegrationEvent
{
    public Guid Id { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
