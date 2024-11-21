// using KiloMart.Core.Authentication;
// using KiloMart.Core.Contracts;
// using KiloMart.Core.Models;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Orders.Shared;

// namespace KiloMart.Domain.Orders.Step3.Reject;
// public static class DeliveryRejectOrderService
// {
//     public static async Task<Result<bool>> Reject(
//         IDbFactory dbFactory,
//         UserPayLoad userPayLoad,
//         long id)
//     {
//         // insert to db
//         using var connection = dbFactory.CreateDbConnection();
//         using var readConnection = dbFactory.CreateDbConnection();
//         connection.Open();
//         readConnection.Open();
//         using var transaction = connection.BeginTransaction();
//         try
//         {
//             var order = await Db.GetOrderByIdAsync(id, readConnection);
//             if (order is null)
//             {
//                 return Result<bool>.Fail(["Not Found"]);
//             }
//             await Db.UpdateOrderAsync
//                 (connection,
//                 id, (byte)OrderStatus.RejectedFromDelivery,
//                 order.TotalPrice,
//                 order.TransactionId,
//                 order.CustomerLocation,
//                 order.ProviderLocation,
//                 order.Customer,
//                 order.Provider,
//                 transaction);

//             OrderActivity orderActivity = new()
//             {
//                 Date = DateTime.Now,
//                 OperatedBy = userPayLoad.Party,
//                 Order = id,
//                 OrderActivityType = (byte)OrderActivityType.RejectedFromDelivery
//             };

//             orderActivity.Id = await Db.InsertOrderActivityAsync(connection,
//                 orderActivity.Order,
//                 orderActivity.Date,
//                 orderActivity.OrderActivityType,
//                 orderActivity.OperatedBy,
//                 transaction);

//             transaction.Commit();
//             return Result<bool>.Ok(true);
//         }
//         catch (Exception exception)
//         {
//             transaction.Rollback();
//             return Result<bool>.Fail([exception.Message]);
//         }

//     }
// }