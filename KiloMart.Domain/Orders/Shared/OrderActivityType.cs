namespace KiloMart.Domain.Orders.Shared;

public enum OrderActivityType
{
    InitByCustomer = 1,
    AcceptedFromProvider = 2,
    Shipped,
    Delivered,
    Cancelled
}

