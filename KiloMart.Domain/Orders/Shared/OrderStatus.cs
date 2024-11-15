namespace KiloMart.Domain.Orders.Shared;

public enum OrderStatus
{
    Initiated = 1,
    AcceptedFromProvider = 2,
    RejectedFromProvider = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6
}

