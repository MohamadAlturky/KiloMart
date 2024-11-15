namespace KiloMart.Domain.Orders.Shared;

public enum OrderActivityType
{
    InitByCustomer = 1,
    AcceptedFromProvider = 2,
    RejectedFromProvider = 3,
    Shipped,
    Delivered,
    Cancelled
}

