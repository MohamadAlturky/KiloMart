namespace KiloMart.Domain.Orders.Common;

public enum OrderStatus
{
    Init = 1,
    Canceled = 2,
    Accepted = 3,
    Delivered = 4
}



public enum OrderActivityType
{
    InitByCustomer = 1,
    Canceled = 2,
    Accepted = 3,
    Delivered = 4
}


