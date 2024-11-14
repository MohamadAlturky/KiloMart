namespace KiloMart.Domain.Orders.Shared;

public enum OrderActivityType
{
    InitByCustomer = 1,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}

