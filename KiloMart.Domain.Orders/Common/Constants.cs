namespace KiloMart.Domain.Orders.Common;
public enum OrderStatus {
    ORDER_PLACED = 1,
    PREPARING = 2,
    SHIPPED = 3,
    CANCELED = 4,
    COMPLETED = 5
}


// INSERT INTO OrderStatus (Id, Name)
// VALUES 
//     (1, 'Order Placed'),
//     (2, 'Preparing'),
//     (3, 'Shipped'),
//     (4, 'Canceled'),
//     (5, 'Completed');



public enum OrderActivityType
{
    InitByCustomer = 1,
    AcceptedByProvider = 2,
    Canceled = 2,
    Accepted = 3,
    Delivered = 4
}


