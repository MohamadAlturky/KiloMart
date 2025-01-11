using System.Data;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Admin;
using KiloMart.Domain.Orders.Common;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Admin;

[ApiController]
[Route("api/admin-panel")]
public class AdminPanelController : AppController
{
    private readonly IWebHostEnvironment _environment;

    public AdminPanelController(IDbFactory dbFactory, IUserContext userContext, IWebHostEnvironment environment) : base(dbFactory, userContext)
    {
        _environment = environment;
    }

    [HttpGet("order-count")]
    public async Task<IActionResult> GetOrderCount()
    {

        using var connection = _dbFactory.CreateDbConnection();
        long canecledOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.CANCELED);
        long completedOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.COMPLETED);
        long OrdersPlacedCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.ORDER_PLACED);
        long shippedOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.SHIPPED);
        long preparingOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.PREPARING);

        long totalSystemFees = await Stats.GetSystemFeeSumByStatusAsync(connection, (byte)OrderStatus.COMPLETED);

        return Success(new
        {
            canecledOrdersCount,
            completedOrdersCount,
            OrdersPlacedCount,
            shippedOrdersCount,
            preparingOrdersCount,
            totalOrdersCount = canecledOrdersCount + completedOrdersCount + OrdersPlacedCount + shippedOrdersCount + preparingOrdersCount,
            Profit = totalSystemFees
        });

    }


    [HttpGet("users-count")]
    public async Task<IActionResult> GetUsersCount()
    {

        using var connection = _dbFactory.CreateDbConnection();

        // Get customer counts
        long activeCustomers = await Stats.GetActiveCustomersCountAsync(connection);
        long deletedCustomers = await Stats.GetDeletedCustomersCountAsync(connection);

        // Get provider counts
        long activeProviders = await Stats.GetActiveProvidersCountAsync(connection);
        long deletedProviders = await Stats.GetDeletedProvidersCountAsync(connection);

        // Get delivery counts
        long activeDeliveries = await Stats.GetActiveDeliveriesCountAsync(connection);
        long deletedDeliveries = await Stats.GetDeletedDeliveriesCountAsync(connection);

        return Success(new
        {
            customers = new { active = activeCustomers, deleted = deletedCustomers, total = activeCustomers + deletedCustomers },
            providers = new { active = activeProviders, deleted = deletedProviders, total = activeProviders + deletedProviders },
            deliveries = new { active = activeDeliveries, deleted = deletedDeliveries, total = activeDeliveries + deletedDeliveries },
            totalUsers = activeCustomers + deletedCustomers + activeProviders + deletedProviders + activeDeliveries + deletedDeliveries
        });

    }
    [HttpGet("offers-count")]
    public async Task<IActionResult> GetOffersCount()
    {

        using var connection = _dbFactory.CreateDbConnection();

        long offersCount = await Stats.GetActiveProductOffersCount(connection);

        return Success(new
        {
            offersCount
        });
    }
    [HttpGet("order-users-offers-count")]
    public async Task<IActionResult> GetStatsSummary()
    {

        using var connection = _dbFactory.CreateDbConnection();

        long offersCount = await Stats.GetActiveProductOffersCount(connection);


        // Get customer counts
        long activeCustomers = await Stats.GetActiveCustomersCountAsync(connection);
        long deletedCustomers = await Stats.GetDeletedCustomersCountAsync(connection);

        // Get provider counts
        long activeProviders = await Stats.GetActiveProvidersCountAsync(connection);
        long deletedProviders = await Stats.GetDeletedProvidersCountAsync(connection);

        // Get delivery counts
        long activeDeliveries = await Stats.GetActiveDeliveriesCountAsync(connection);
        long deletedDeliveries = await Stats.GetDeletedDeliveriesCountAsync(connection);



        long canecledOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.CANCELED);
        long completedOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.COMPLETED);
        long OrdersPlacedCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.ORDER_PLACED);
        long shippedOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.SHIPPED);
        long preparingOrdersCount = await Stats.GetOrderCountByStatusAsync(connection, (byte)OrderStatus.PREPARING);

        long totalSystemFees = await Stats.GetSystemFeeSumByStatusAsync(connection, (byte)OrderStatus.COMPLETED);
        return Success(new
        {
            offersCount,
            customers = new { active = activeCustomers, deleted = deletedCustomers, total = activeCustomers + deletedCustomers },
            providers = new { active = activeProviders, deleted = deletedProviders, total = activeProviders + deletedProviders },
            deliveries = new { active = activeDeliveries, deleted = deletedDeliveries, total = activeDeliveries + deletedDeliveries },
            totalUsers = activeCustomers + deletedCustomers + activeProviders + deletedProviders + activeDeliveries + deletedDeliveries,
            canecledOrdersCount,
            completedOrdersCount,
            OrdersPlacedCount,
            shippedOrdersCount,
            preparingOrdersCount,
            totalOrdersCount = canecledOrdersCount + completedOrdersCount + OrdersPlacedCount + shippedOrdersCount + preparingOrdersCount,
            Profit = totalSystemFees
        });
    }
    [HttpGet("order-yearly-stats-summary-by-year")]
    public async Task<IActionResult> GetOrderSummaryAsync(
        [FromQuery] int year
    )
    {

        using var connection = _dbFactory.CreateDbConnection();

        var orderThatPaidByCashStats = await Stats.GetOrderSummaryAsync(
            connection,
            true,
            (byte)PaymentType.Cash,
            year);

        var orderThatPaidByElcetronicStats = await Stats.GetOrderSummaryAsync(
            connection,
            true,
            (byte)PaymentType.Elcetronic,
            year);

        var orderThatNotPaidByCashStats = await Stats.GetOrderSummaryAsync(
           connection,
           false,
           (byte)PaymentType.Cash,
           year);

        var orderThatNotPaidByElcetronicStats = await Stats.GetOrderSummaryAsync(
            connection,
            false,
            (byte)PaymentType.Elcetronic,
            year);

        return Success(new
        {
            orderThatNotPaidByElcetronicStats,
            orderThatNotPaidByCashStats,
            orderThatPaidByElcetronicStats,
            orderThatPaidByCashStats
        });
    }
    [HttpGet("order-yearly-stats-summary-by-month")]
    public async Task<IActionResult> GetOrderSummaryMonthlyAsync(
        [FromQuery] int year,
        [FromQuery] int month
    )
    {

        using var connection = _dbFactory.CreateDbConnection();

        var orderThatPaidByCashStats = await Stats.GetOrderSummaryAsync(
            connection,
            true,
            (byte)PaymentType.Cash,
            year,
            month);

        var orderThatPaidByElcetronicStats = await Stats.GetOrderSummaryAsync(
            connection,
            true,
            (byte)PaymentType.Elcetronic,
            year);

        var orderThatNotPaidByCashStats = await Stats.GetOrderSummaryAsync(
           connection,
           false,
           (byte)PaymentType.Cash,
           year);

        var orderThatNotPaidByElcetronicStats = await Stats.GetOrderSummaryAsync(
            connection,
            false,
            (byte)PaymentType.Elcetronic,
            year);

        return Success(new
        {
            orderThatNotPaidByElcetronicStats,
            orderThatNotPaidByCashStats,
            orderThatPaidByElcetronicStats,
            orderThatPaidByCashStats
        });
    }

    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////
    /////////////////////////////


    [HttpGet("orders-count-yearly-stats-summary-by-month")]
    public async Task<IActionResult> GetOrderCountSummaryAsync(
        [FromQuery] int? year,
        [FromQuery] int? month
    )
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetching order statistics for the specified year and month
        var orderStats = await GetOrderStatisticsAsync(connection, year, month);

        return Success(orderStats);
    }

    private async Task<object> GetOrderStatisticsAsync(IDbConnection connection, int? year, int? month)
    {
        var orderStatuses = new[]
        {
            OrderStatus.CANCELED,
            OrderStatus.COMPLETED,
            OrderStatus.PREPARING,
            OrderStatus.SHIPPED,
            OrderStatus.ORDER_PLACED
        };
        var paymentTypes = new[] { PaymentType.Cash, PaymentType.Elcetronic };

        var results = new Dictionary<string, List<OrderCountMonthlySummary>>();

        foreach (var status in orderStatuses)
        {
            foreach (var paymentType in paymentTypes)
            {
                byte orderStatus = (byte)status;

                bool? isPaid = false;
                var stats = await Stats.GetOrderCountSummaryAsync(
                    connection,
                    isPaid,
                    (byte)paymentType,
                    year,
                    month,
                    orderStatus
                );

                results[$"{status}_{paymentType}_Paid"] = stats;


                isPaid = false;
                var statsNotPaid = await Stats.GetOrderCountSummaryAsync(
                    connection,
                    isPaid,
                    (byte)paymentType,
                    year,
                    month,
                    orderStatus
                );

                results[$"{status}_{paymentType}_Not_Paid"] = statsNotPaid;
            }
        }

        return new
        {
            results
        };
    }




    [HttpGet("orders-count-summary")]
    public async Task<IActionResult> GetOrdersCountSummaryAsync()
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetching various order statistics
        var canceledOrderCount = await Stats.GetCanceledOrderCountAsync(connection);
        var completedOrderCount = await Stats.GetCompletedOrderCountAsync(connection);
        var totalOrderCount = await Stats.GetTotalOrderCountAsync(connection);
        var noProviderCount = await Stats.ThereIsNoProviderAcceptedItAsync(connection);
        var noDeliveryCount = await Stats.ThereIsNoDeliveryAcceptedItAsync(connection);

        // Create a summary object to hold the results
        var orderSummary = new
        {
            CanceledOrders = canceledOrderCount,
            CompletedOrders = completedOrderCount,
            TotalOrders = totalOrderCount,
            NoProviderAccepted = noProviderCount,
            NoDeliveryAccepted = noDeliveryCount
        };

        return Success(orderSummary);
    }
    [HttpGet("providers-summary")]
    public async Task<IActionResult> GetProvidersSummaryAsync()
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetching provider statistics
        var stats = await Stats.GetStatisticsAsync(connection);

        // Create a summary object to hold the results
        var providersSummary = new
        {
            TotalProviders = stats.TotalProviders,
            TotalProvidersBalance = stats.TotalProvidersBalance,
            TotalProductOffers = stats.TotalProductOffers,
            TotalActiveProviders = stats.ActiveProviders
        };

        return Ok(providersSummary); // Return a successful response with the summary
    }
    [HttpGet("providers/paginated")]
    public async Task<IActionResult> GetPaginatedProvidersAsync(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedProvidersAsync(connection, page, pageSize);

        return Ok(new
        {
            result.TotalCount,
            providers = result.Data.Select(e =>
            {
                return
                new
                {
                    e.ProviderId,
                    e.DisplayName,
                    e.Email,
                    e.PhoneNumber,
                    e.IsActive,
                    e.TotalOrders,
                    e.TotalProducts,
                    e.TotalBalance,
                    locationDetails = new
                    {
                        e.Long,
                        e.Lat,
                        e.City,
                        e.BuildingNumber,
                        e.ApartmentNumber,
                        e.FloorNumber,
                        e.StreetNumber,
                    }
                };
            }).ToList()
        }); // Return a successful response with the paginated data
    }
}