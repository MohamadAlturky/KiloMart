using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Shared;

namespace KiloMart.Domain.Orders.Step1;

public class OrderFactory
{
    public static async Task<Result<DomainOrder>> Create(
        IDbFactory dbFactory,
        CreateOrderRequest request,
        int customer)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        
        if(request.OrderItems.Count == 0)
        {
            return Result<DomainOrder>.Fail([$"can't create empty order"]);
        }
        List<ProductOffer> offers = [];
        List<OrderItem> orderItems = [];

        foreach (var item in request.OrderItems)
        {
            ProductOffer? offer = await Db.GetProductOfferByIdAsync(item.ProductOffer, connection);
            
            if (offer is null)
            {
                return Result<DomainOrder>.Fail([$"product offer with id = {item.ProductOffer} not found"]);
            }
            if (offer.IsActive == false)
            {
                return Result<DomainOrder>.Fail([$"product offer with id = {item.ProductOffer} not found not active"]);
            }          
            if (offer.Quantity < item.Quantity)
            {
                return Result<DomainOrder>.Fail([$"product offer with id = {item.ProductOffer} don't have enough quantity the requested quantity is {item.Quantity} and the stock is {offer.Quantity}"]);
            }
            orderItems.Add(new()
            {
                ProductOffer = offer.Id,
                Quantity = item.Quantity,
                UnitPrice = offer.Price,
                Order = 0
            });
            offers.Add(offer);
        }

        int provider = offers[0].Provider; 
        foreach (var offer in offers)
        {
            if(offer.Provider != provider)
            {
                return Result<DomainOrder>.Fail([$"can't order form multiple providers"]);
            }
        }

        return Result<DomainOrder>.Ok(new DomainOrder
        {
            Order = new()
            {
                Provider = provider,
                Customer = customer,
                CustomerLocation = request.CustomerLocation,
                ProviderLocation = request.ProviderLocation,
                TransactionId = Guid.NewGuid().ToString(),
                TotalPrice = orderItems.Sum(x => ((decimal)x.Quantity) * x.UnitPrice),
                OrderStatus = (byte)OrderStatus.Initiated
            },
            Items = orderItems
        });
    }
}