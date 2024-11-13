namespace KiloMart.Domain.Orders.Models;

public enum OrderActivityTypes
{
    InitByCustomer = 1,
    CanceledByUserBeforeProviderAcceptIt = 2,
    ProviderAcceptedTheOrder = 3,
    ProviderCancelledTheOrderBeforeTheDelivaryAcceptIt = 4,
    
}