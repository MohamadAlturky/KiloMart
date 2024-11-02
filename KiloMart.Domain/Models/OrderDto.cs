namespace KiloMart.Domain.Models;

public class OrderDto
{
    public long Id { get; set; }
    public byte OrderStatus { get; set; }
    public decimal TotalPrice { get; set; }
}
