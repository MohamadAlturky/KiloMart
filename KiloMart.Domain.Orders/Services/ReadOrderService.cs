using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Orders.Helpers;
using KiloMart.Domain.Orders.Repositories;

namespace KiloMart.Domain.Orders.Services;
public static class ReadOrderService
{
    public static async Task<Result<List<AggregatedOrder>>> GetMineByStatusAsync(
        byte language,
        byte status,
        IUserContext userContext,
        IDbFactory dbFactory)
    {
        try
        {
            var partyId = userContext.Get().Party;

            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            var whereClause = "WHERE Customer = @customer AND OrderStatus = @status";
            var parameters = new { customer = partyId, status };

            // Get the orders
            var orders = await OrderRepository.GetOrderDetailsAsync(connection, whereClause, parameters);

            if(orders is null)
            {
                return Result<List<AggregatedOrder>>.Ok([]);
            }
            if(!orders.Any())
            {
                return Result<List<AggregatedOrder>>.Ok([]);
            }
            var ordersIds = orders.Select(e => e.Id).ToList();

            // Get the orders products
            var ordersProducts = await OrderRepository.GetOrderProductsByIdsAsync(connection, ordersIds, language);

            // Get the orders products offers
            var ordersOffersProducts = await OrderRepository.GetOrderProductOffersByIdsAsync(connection, ordersIds, language);

            List<AggregatedOrder> aggregatedOrders = OrderAggregator.Aggregate(
                orders.AsList(),
                ordersProducts.AsList(),
                ordersOffersProducts.AsList());

            return Result<List<AggregatedOrder>>.Ok(aggregatedOrders);
        }
        catch (Exception e)
        {
            return Result<List<AggregatedOrder>>.Fail([e.Message]);
        }
    }
}