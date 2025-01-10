using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Admin;
using KiloMart.Domain.Orders.Common;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Admin
{
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
        [HttpGet("order-yearly-stats-summary")]
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
    }
}