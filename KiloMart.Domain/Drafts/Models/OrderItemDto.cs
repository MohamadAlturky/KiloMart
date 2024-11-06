namespace KiloMart.Domain.Models;

public class OrderItemDto
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public int ProductOfferId { get; set; }
    public decimal Price { get; set; }
}
