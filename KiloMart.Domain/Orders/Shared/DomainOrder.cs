using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Step1;

namespace KiloMart.Domain.Orders.Shared;


public class DomainOrder
{
    public Order Order { get; set; } = new();
    public List<OrderItem>  Items { get; set; } = [];
    public OrderActivity OrderActivity { get; set; } = new();
    public List<OrderItemRequest> Skipped = [];
}