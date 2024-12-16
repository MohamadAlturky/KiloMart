using Dapper;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.Helpers;
using KiloMart.Domain.Orders.Repositories;

namespace KiloMart.Domain.Orders.Services;
public static class ReadOrderService
{
    #region customer
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

            if (orders is null)
            {
                return Result<List<AggregatedOrder>>.Ok([]);
            }
            if (!orders.Any())
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
    #endregion

    #region provider

    public static async Task<Result<List<AggregatedOrder>>> GetMineByStatusForProviderAsync(
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

            var whereClause = "WHERE Provider = @provider AND OrderStatus = @status";
            var parameters = new { provider = partyId, status };

            // Get the orders
            var orders = await OrderRepository.GetOrderDetailsAsync(connection, whereClause, parameters);

            if (orders is null)
            {
                return Result<List<AggregatedOrder>>.Ok([]);
            }
            if (!orders.Any())
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

    public static async Task<Result<List<AggregatedOrder>>> GetRequestedOrders(
        byte language,
        IDbFactory dbFactory)
    {
        try
        {
            byte status = (byte)OrderStatus.ORDER_PLACED;
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            var whereClause = "WHERE OrderStatus = @status";
            var parameters = new { status };

            // Get the orders
            var orders = await OrderRepository.GetOrderDetailsAsync(connection, whereClause, parameters);

            if (orders is null)
            {
                return Result<List<AggregatedOrder>>.Ok([]);
            }
            if (!orders.Any())
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
    public static async Task<Result<List<AggregatedOrder>>> GetPreparingOrders(
        byte language,
        int providerId,
        IDbFactory dbFactory)
    {
        try
        {
            byte status = (byte)OrderStatus.PREPARING;
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            var whereClause = "WHERE OrderStatus = @status AND opi.Provider = @providerId";
            var parameters = new { status, providerId };

            // Get the orders
            IEnumerable<OrderDetailsDto>? orders = await OrderRepository.GetOrderDetailsAsync(connection, whereClause, parameters);

            if (orders is null)
            {
                return Result<List<AggregatedOrder>>.Ok([]);
            }
            if (!orders.Any())
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
    #endregion

    #region Delivery
    public static async Task<Result<List<OrderDetailsDto>>> GetReadyForDeliverAsync(
        IUserContext userContext,
        IDbFactory dbFactory)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();
            byte status = (byte)OrderStatus.PREPARING;
            var whereClause = "WHERE OrderStatus = @status AND Delivery is NULL";
            var parameters = new { status };

            // Get the orders
            IEnumerable<OrderDetailsDto>? orders = await OrderRepository.GetOrderDetailsAsync(connection, whereClause, parameters);

            if (orders is null)
            {
                return Result<List<OrderDetailsDto>>.Ok([]);
            }
            if (!orders.Any())
            {
                return Result<List<OrderDetailsDto>>.Ok([]);
            }

            return Result<List<OrderDetailsDto>>.Ok(orders.AsList());
        }
        catch (Exception e)
        {
            return Result<List<OrderDetailsDto>>.Fail([e.Message]);
        }
    }
    public static async Task<Result<List<OrderDetailsDto>>> GetCompletedForDeliveryAsync(
        IUserContext userContext,
        IDbFactory dbFactory)
    {
        try
        {
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();
            byte status = (byte)OrderStatus.COMPLETED;
            int deliveryId = userContext.Get().Party;

            var whereClause = "WHERE OrderStatus = @status AND Delivery = @delivery";
            var parameters = new { status, delivery = deliveryId };

            // Get the orders
            IEnumerable<OrderDetailsDto>? orders = await OrderRepository.GetOrderDetailsAsync(connection, whereClause, parameters);

            if (orders is null)
            {
                return Result<List<OrderDetailsDto>>.Ok([]);
            }
            if (!orders.Any())
            {
                return Result<List<OrderDetailsDto>>.Ok([]);
            }

            return Result<List<OrderDetailsDto>>.Ok(orders.AsList());
        }
        catch (Exception e)
        {
            return Result<List<OrderDetailsDto>>.Fail([e.Message]);
        }
    }
    
    public static async Task<Result<List<AggregatedOrder>>> GetShippingOrders(
        byte language,
        int deliveryId,
        IDbFactory dbFactory)
    {
        try
        {
            byte status = (byte)OrderStatus.SHIPPED;
            using var connection = dbFactory.CreateDbConnection();
            connection.Open();

            var whereClause = "WHERE OrderStatus = @status AND odi.Delivery = @deliveryId";
            var parameters = new { status, deliveryId };

            // Get the orders
            IEnumerable<OrderDetailsDto>? orders = await OrderRepository.GetOrderDetailsAsync(connection, whereClause, parameters);

            if (orders is null)
            {
                return Result<List<AggregatedOrder>>.Ok([]);
            }
            if (!orders.Any())
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
    
    
    
    #endregion
}