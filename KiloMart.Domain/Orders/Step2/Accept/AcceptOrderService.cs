// using KiloMart.Core.Authentication;
// using KiloMart.Core.Contracts;
// using KiloMart.Core.Models;
// using KiloMart.DataAccess.Database;
// using KiloMart.Domain.Orders.Shared;

// namespace KiloMart.Domain.Orders.Step2.Accept;
// public static class AcceptOrderService
// {
//     public static async Task<Result<bool>> Accept(
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
//             if (order.Provider != userPayLoad.Party)
//             {
//                 return Result<bool>.Fail(["Un Authorized"]);
//             }
//             await Db.UpdateOrderAsync
//                 (connection,
//                 id, (byte)OrderStatus.AcceptedFromProvider,
//                 order.TotalPrice,
//                 order.TransactionId,
//                 order.CustomerLocation,
//                 order.ProviderLocation,
//                 order.Customer,
//                 order.Provider,
//                 transaction);

//             OrderActivity orderActivity = new()
//             {
//                 Date = SaudiDateTimeHelper.GetCurrentTime(),
//                 OperatedBy = userPayLoad.Party,
//                 Order = id,
//                 OrderActivityType = (byte)OrderActivityType.AcceptedFromProvider
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