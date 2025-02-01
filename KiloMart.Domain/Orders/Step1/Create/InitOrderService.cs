// using KiloMart.Core.Authentication;
// using KiloMart.Core.Contracts;
// using KiloMart.Core.Models;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Orders.Shared;
// using OrderActivity = KiloMart.DataAccess.Database.OrderActivity;

// namespace KiloMart.Domain.Orders.Step1.Create;
// public static class InitOrderService
// {
//     public static async Task<Result<DomainOrder>> Insert(
//         IDbFactory dbFactory,
//         UserPayLoad userPayLoad,
//         CreateOrderRequest model)
//     {
//         var (success, errors) = model.Validate();
//         if (!success)
//         {
//             return Result<DomainOrder>.Fail(errors);
//         }
//         // buissness logic
//         Result<DomainOrder> orderCreationResult = await OrderFactory.Create(dbFactory, model, userPayLoad.Party);
//         if (!orderCreationResult.Success)
//         {
//             return Result<DomainOrder>.Fail(orderCreationResult.Errors);
//         }

//         // insert to db
//         using var connection = dbFactory.CreateDbConnection();
//         connection.Open();
//         using var transaction = connection.BeginTransaction();
//         try
//         {
//             long orderId = await Db.InsertOrderAsync(connection,
//                         orderCreationResult.Data.Order.OrderStatus,
//                         orderCreationResult.Data.Order.TotalPrice,
//                         orderCreationResult.Data.Order.TransactionId,
//                         orderCreationResult.Data.Order.CustomerLocation,
//                         orderCreationResult.Data.Order.ProviderLocation,
//                         orderCreationResult.Data.Order.Customer,
//                         orderCreationResult.Data.Order.Provider,
//                         transaction);

//             foreach (var item in orderCreationResult.Data.Items)
//             {
//                 item.Order = orderId;
//                 item.Id = await Db.InsertOrderItemAsync(connection,
//                 item.Order,
//                 item.ProductOffer,
//                 item.UnitPrice,
//                 item.Quantity,
//                 transaction);
//             }
//             OrderActivity orderActivity = new()
//             {
//                 Date = SaudiDateTimeHelper.GetCurrentTime(),
//                 OperatedBy = userPayLoad.Party,
//                 Order = orderId,
//                 OrderActivityType = (byte)OrderActivityType.InitByCustomer
//             };

//             orderActivity.Id = await Db.InsertOrderActivityAsync(connection,
//                 orderActivity.Order,
//                 orderActivity.Date,
//                 orderActivity.OrderActivityType,
//                 orderActivity.OperatedBy,
//                 transaction);

//             orderCreationResult.Data.OrderActivity = orderActivity;

//             transaction.Commit();
//             return Result<DomainOrder>.Ok(orderCreationResult.Data);
//         }
//         catch (Exception exception)
//         {
//             transaction.Rollback();
//             return Result<DomainOrder>.Fail([exception.Message]);
//         }

//     }
// }