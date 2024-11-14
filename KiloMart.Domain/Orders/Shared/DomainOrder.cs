using KiloMart.DataAccess.Database;

namespace KiloMart.Domain.Orders.Shared;


public class DomainOrder
{
    public Order Order { get; set; } = new();
    public List<OrderItem>  Items { get; set; } = [];
    public OrderActivity OrderActivity { get; set; } = new();
}