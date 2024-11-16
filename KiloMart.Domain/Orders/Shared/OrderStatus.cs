namespace KiloMart.Domain.Orders.Shared;

public enum OrderStatus
{
    Initiated = 1,
    AcceptedFromProvider = 2,
    RejectedFromProvider = 3,
    RejectedFromDelivery = 4,
    AcceptedFromDelivery = 5,

    Cancelled = 6
}

