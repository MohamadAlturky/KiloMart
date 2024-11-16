namespace KiloMart.Domain.Orders.Shared;

public enum OrderActivityType
{
    InitByCustomer = 1,
    AcceptedFromProvider = 2,
    RejectedFromProvider = 3,
    CancelledByCustomer = 4,
    AcceptedFromDelivary = 5,

    Shipped,
    Delivered,
    Cancelled,
    RejectedFromDelivery = 6
}

