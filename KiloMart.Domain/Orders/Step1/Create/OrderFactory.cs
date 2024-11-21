// using KiloMart.Core.Contracts;
// using KiloMart.Core.Models;
// using KiloMart.Core.Settings;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Orders.Shared;

// namespace KiloMart.Domain.Orders.Step1.Create;

// public static class OrderFactory
// {
//     public static int GetMostCommonProvider(List<ProductOffer> productOffers)
//     {
//         if (productOffers == null || productOffers.Count == 0)
//         {
//             throw new ArgumentException("Product offers list cannot be empty or null.");
//         }

//         IGrouping<int, ProductOffer>? mostCommonProvider = productOffers
//             .GroupBy(p => p.Provider)
//             .OrderByDescending(g => g.Count())
//             .FirstOrDefault();

//         if (mostCommonProvider is null)
//         {
//             throw new Exception("Error in GetMostCommonProvider");
//         }

//         return mostCommonProvider.Key;
//     }
//     public static async Task<Result<DomainOrder>> Create(
//         IDbFactory dbFactory,
//         CreateOrderRequest request,
//         int customer)
//     {
//         using var connection = dbFactory.CreateDbConnection();
//         connection.Open();

//         if (request.OrderItems.Count == 0)
//         {
//             return Result<DomainOrder>.Fail([$"can't create empty order"]);
//         }
//         List<ProductOffer> offers = [];
//         List<OrderItem> orderItems = [];

//         foreach (var item in request.OrderItems)
//         {
//             ProductOffer? offer = await Db.GetProductOfferByIdAsync(item.ProductOffer, connection);

//             if (offer is null)
//             {
//                 return Result<DomainOrder>.Fail([$"product offer with id = {item.ProductOffer} not found"]);
//             }
//             if (offer.IsActive == false)
//             {
//                 return Result<DomainOrder>.Fail([$"product offer with id = {item.ProductOffer} not found not active"]);
//             }
//             if (offer.Quantity < item.Quantity)
//             {
//                 return Result<DomainOrder>.Fail([$"product offer with id = {item.ProductOffer} don't have enough quantity the requested quantity is {item.Quantity} and the stock is {offer.Quantity}"]);
//             }
//             offers.Add(offer);
//         }

//         int provider = offers[0].Provider;
//         bool orderedFromMultipleProviders = false;
//         foreach (var offer in offers)
//         {
//             if (offer.Provider != provider)
//             {
//                 orderedFromMultipleProviders = true;
//             }
//         }
//         //if (orderedFromMultipleProviders && ConstantSettings.CancelWhenOrderFromMultiProviders)
//         //{
//         //    return Result<DomainOrder>.Fail([$"can't order form multiple providers"]);
//         //}
//         //if (orderedFromMultipleProviders && !ConstantSettings.CancelWhenOrderFromMultiProviders)
//         //{

//         //}
//         int mostOccuredProvider = GetMostCommonProvider(offers);
//         Location? providerLocation = await Db.GetLocationByPartyAsync(mostOccuredProvider, connection);
//         if (providerLocation is null)
//         {
//             return Result<DomainOrder>.Fail([$"provider don't have location"]);
//         }
//         offers = offers.Where(e => e.Provider == mostOccuredProvider).ToList();
//         List<OrderItemRequest> skipped = [];
//         foreach (var item in request.OrderItems)
//         {
//             ProductOffer? offer = offers.FirstOrDefault(e => e.Id == item.ProductOffer);

//             if (offer is null)
//             {
//                 skipped.Add(item);
//             }
//             else
//             {

//                 orderItems.Add(new()
//                 {
//                     ProductOffer = offer.Id,
//                     Quantity = item.Quantity,
//                     UnitPrice = offer.Price,
//                     Order = 0
//                 });
//             }
//         }
//         return Result<DomainOrder>.Ok(new DomainOrder
//         {
//             Skipped = skipped,
//             Order = new()
//             {
//                 Provider = provider,
//                 Customer = customer,
//                 CustomerLocation = request.CustomerLocation,
//                 ProviderLocation = providerLocation.Id,
//                 TransactionId = Guid.NewGuid().ToString(),
//                 TotalPrice = orderItems.Sum(x => (decimal)x.Quantity * x.UnitPrice),
//                 OrderStatus = (byte)OrderStatus.Initiated
//             },
//             Items = orderItems
//         });
//     }
// }
