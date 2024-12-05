namespace KiloMart.Domain.Orders.Common;
public enum OrderStatus
{
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
    /// <summary>
    /// Customer 
    /// </summary>
    InitByCustomer = 1,
    CanceledByCustomerBeforeProviderAcceptIt = 2,
    CanceledByCustomerBeforeDeliveryAcceptItAndAfterProviderAcceptIt = 3,
    CanceledByCustomerAfterDeliveryAcceptIt = 4,

    /// <summary>
    /// Provider
    /// </summary>
    AcceptedByProvider = 5,
    CanceledByProviderBeforeDeliveryAcceptIt = 6,

    /// <summary>
    /// Delivery
    /// </summary>
    AcceptedByDelivery = 7,
    CanceledByDelivery = 8,
    ShippedByDelivery = 9,
    DeliveredByDelivery = 10
}

// INSERT INTO OrderActivityType([Id], [Name])
// VALUES 
//     (1, 'InitByCustomer'),
//     (2, 'CanceledByCustomerBeforeProviderAcceptIt'),
//     (3, 'CanceledByCustomerBeforeDeliveryAcceptAfterProviderAccept'),
//     (4, 'CanceledByCustomerAfterDeliveryAcceptIt'),
//     (5, 'AcceptedByProvider'),
//     (6, 'CanceledByProviderBeforeDeliveryAcceptIt'),
//     (7, 'AcceptedByDelivery'),
//     (8, 'CanceledByDelivery'),
//     (9, 'ShippedByDelivery'),
//     (10, 'DeliveredByDelivery');


public enum PaymentType
{
    Cash = 1,
    Elcetronic = 2
}

//  INSERT INTO PaymentType([Id], [Name])
//  VALUES 
//      (1, 'Cash'),
//      (2, 'Elcetronic')
