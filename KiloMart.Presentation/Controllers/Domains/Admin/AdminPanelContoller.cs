using System.Data;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Admin;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.Register.Provider.Services;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Services;
using KiloMart.Presentation.Tracking;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;
using static KiloMart.Presentation.Tracking.DriversTrackerService;

namespace KiloMart.Presentation.Controllers.Domains.Admin;

[ApiController]
[Route("api/admin-panel")]
public class AdminPanelController : AppController
{
    private readonly IWebHostEnvironment _environment;
    private readonly DriversTrackerService _driversTrackerService;

    public AdminPanelController(DriversTrackerService driversTrackerService, IDbFactory dbFactory, IUserContext userContext, IWebHostEnvironment environment) : base(dbFactory, userContext)
    {
        _environment = environment;
        _driversTrackerService = driversTrackerService;
    }
    #region some stats
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
    #endregion

    #region provider
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
                    e.WithdrawalBalance,
                    e.ReceivedBalance,
                    TotalBalnace = e.ReceivedBalance,
                    AvailableBalnace = e.ReceivedBalance - e.WithdrawalBalance,
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
    [HttpGet("providers/paginated-by-search-term")]
    public async Task<IActionResult> GetPaginatedProvidersByTermAsync(
        [FromQuery] string? term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedProvidersDataAsync(connection, page, pageSize, term);
        var count = await Stats.GetActiveFilteredProvidersProfilesCountAsync(connection, term);

        return Ok(new
        {
            TotalCount = count,
            providers = result.Select(e =>
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
                    e.WithdrawalBalance,
                    e.ReceivedBalance,
                    TotalBalnace = e.ReceivedBalance,
                    AvailableBalnace = e.ReceivedBalance - e.WithdrawalBalance,
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

    [HttpPost("create-provider-directly")]
    public async Task<IActionResult> Insert(AdminInsertProviderModel request)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        #region 
        var result = await new RegisterProviderService().RegisterDirectly(
            connection,
            transaction,
            request.Email,
            request.Password,
            request.DisplayName,
            request.Language);
        #endregion

        if (!result.IsSuccess)
        {
            return Fail("can't create user info");
        }
        if (!result.PartyId.HasValue)
        {
            return Fail("can't create user info");
        }
        try
        {
            #region File Uploads

            // OwnershipDocumentFile
            if (request.OwnershipDocumentFile == null)
            {
                return Fail("OwnershipDocumentFile is null");
            }
            var ownershipDocumentFilePath = await FileService.SaveImageFileAsync(
                request.OwnershipDocumentFile,
                _environment.WebRootPath,
                Guid.NewGuid());
            if (string.IsNullOrEmpty(ownershipDocumentFilePath))
            {
                return Fail("Failed to save OwnershipDocumentFile");
            }

            // OwnerNationalApprovalFile
            if (request.OwnerNationalApprovalFile == null)
            {
                return Fail("OwnerNationalApprovalFile is null");
            }
            var ownerNationalApprovalFilePath = await FileService.SaveImageFileAsync(
                request.OwnerNationalApprovalFile,
                _environment.WebRootPath,
                Guid.NewGuid());
            if (string.IsNullOrEmpty(ownerNationalApprovalFilePath))
            {
                return Fail("Failed to save OwnerNationalApprovalFile");
            }

            #endregion

            #region Insert into Database
            var now = DateTime.Now;
            long id = await Db.InsertProviderProfileHistoryAsync(
                connection,
                request.FirstName,
                request.SecondName,
                request.NationalApprovalId,
                request.CompanyName,
                request.OwnerName,
                request.OwnerNationalId,
                ownershipDocumentFilePath,
                ownerNationalApprovalFilePath,
                request.LocationName,
                request.Longitude,
                request.Latitude,
                request.BuildingType,
                request.BuildingNumber,
                request.FloorNumber,
                request.ApartmentNumber,
                request.StreetNumber,
                request.PhoneNumber,
                true, // isAccepted
                false, // isRejected
                now, // submitDate
                now, // reviewDate
                result.PartyId.Value, // providerId
                true); // isActive

            #endregion

            #region Returning The Response
            var model = new
            {
                request.FirstName,
                request.SecondName,
                request.NationalApprovalId,
                request.CompanyName,
                request.OwnerName,
                request.OwnerNationalId,
                OwnershipDocumentFileUrl = ownershipDocumentFilePath,
                OwnerNationalApprovalFileUrl = ownerNationalApprovalFilePath,
                request.LocationName,
                request.Longitude,
                request.Latitude,
                request.BuildingType,
                request.BuildingNumber,
                request.FloorNumber,
                request.ApartmentNumber,
                request.StreetNumber,
                request.PhoneNumber,
                IsAccepted = false,
                IsRejected = false,
                SubmitDate = now,
                ReviewDate = now,
                ProviderId = result.PartyId.Value,
                IsActive = true,
                request.DisplayName,
                request.Language
            };


            var locationId = await Db.InsertLocationAsync(
             connection,
             request.Longitude,
             request.Latitude,
             request.LocationName,
             result.PartyId.Value,
             transaction);

            await Db.InsertLocationDetailsAsync(
                connection,
                request.BuildingType,
                request.BuildingNumber,
                request.FloorNumber,
                request.ApartmentNumber,
                request.StreetNumber,
                request.PhoneNumber,
                locationId,
                transaction);
            return Success(new { id, request });
            #endregion
        }
        catch (Exception ex)
        {
            return Fail($"Failed to insert provider profile history: {ex.Message}");
        }
    }


    [HttpGet("provider-by-id")]
    public async Task<IActionResult> GetProviderById(
        [FromQuery] int providerId
    )
    {
        using var connection = _dbFactory.CreateDbConnection();
        var user = await Db.GetMembershipUserByPartyAsync(connection, providerId);
        var party = await Db.GetPartyByIdAsync(providerId, connection);
        var profile = await Db.GetActiveProviderProfileHistoryAsync(connection, providerId);
        var statistics = await Stats.GetProviderStatisticsAsync(connection, providerId);
        return Success(new
        {
            displayName = party?.DisplayName,
            firstName = profile?.FirstName,
            secondName = profile?.SecondName,
            companyName = profile?.CompanyName,
            providerId = providerId,
            userId = user?.Id,
            nationalApprovalId = profile?.NationalApprovalId,
            ownerName = profile?.OwnerName,
            ownerNationalId = profile?.OwnerNationalId,
            email = user?.Email,
            isActive = user?.IsActive,
            totalOrders = statistics is not null ? statistics.TotalOrders : 0,
            totalProducts = statistics is not null ? statistics.TotalProducts : 0,
            ReceivedBalance = statistics is not null ? statistics.ReceivedBalance : 0,
            WithdrawalBalance = statistics is not null ? statistics.WithdrawalBalance : 0,
            availableBalance = (statistics is not null ? statistics.ReceivedBalance : 0) - (statistics is not null ? statistics.WithdrawalBalance : 0),
            totalBalance = statistics is not null ? statistics.ReceivedBalance : 0,
            ownerNationalApprovalFile = profile?.OwnerNationalApprovalFileUrl,
            ownershipDocumentFile = profile?.OwnershipDocumentFileUrl,
            isEmailVerified = user?.EmailConfirmed
        });
    }
    [HttpGet("provider-orders-by-id")]
    public async Task<IActionResult> GetProviderOrdersById(
       [FromQuery] int providerId,
       [FromQuery] byte language
       )
    {
        var result = await ReadOrderService.GeteForProviderAsync(language, providerId, _dbFactory);

        return Success(result.Data.Select(
            e =>
            {
                DriverLocation? location = null;
                if (e.OrderDetails.Delivery.HasValue && e.OrderDetails.OrderStatus == (byte)OrderStatus.SHIPPED)
                {
                    location = _driversTrackerService.GetByKey(e.OrderDetails.Delivery.Value);
                }
                return new
                {
                    OrderDetails = new
                    {
                        e.OrderDetails.Id,
                        OrderStatus = GetOrderStatusFromNumber(e.OrderDetails.OrderStatus).ToString(),
                        e.OrderDetails.TotalPrice,
                        e.OrderDetails.TransactionId,
                        e.OrderDetails.Date,
                        e.OrderDetails.PaymentType,
                        e.OrderDetails.IsPaid,
                        e.OrderDetails.ItemsPrice,
                        e.OrderDetails.SystemFee,
                        e.OrderDetails.DeliveryFee,
                        e.OrderDetails.SpecialRequest,

                        e.OrderDetails.Customer,
                        e.OrderDetails.CustomerLocation,
                        e.OrderDetails.CustomerInformationId,

                        e.OrderDetails.Provider,
                        e.OrderDetails.ProviderLocation,
                        e.OrderDetails.ProviderInformationId,

                        e.OrderDetails.Delivery,
                        e.OrderDetails.DeliveryInformationId,

                        e.OrderDetails.CustomerLocationName,
                        e.OrderDetails.CustomerLocationLatitude,
                        e.OrderDetails.CustomerLocationLongitude,
                        e.OrderDetails.ProviderLocationName,
                        e.OrderDetails.ProviderLocationLatitude,
                        e.OrderDetails.ProviderLocationLongitude,


                        e.OrderDetails.CustomerDisplayName,
                        e.OrderDetails.ProviderDisplayName,
                        e.OrderDetails.DeliveryDisplayName,
                    },
                    e.OrderProductOfferDetails,
                    e.OrderProductDetails,
                    DriverLocation = location
                };
            }
        ).ToList());
    }
    public static OrderStatus? GetOrderStatusFromNumber(int statusNumber)
    {
        // Check if the number corresponds to a valid OrderStatus
        if (Enum.IsDefined(typeof(OrderStatus), statusNumber))
        {
            return (OrderStatus)statusNumber;
        }

        // Return null if the number does not match any OrderStatus
        return null;
    }

    [HttpGet("products-for-provider")]
    public async Task<IActionResult> GetMineByCategory(
           [FromQuery] byte language,
           [FromQuery] int providerId,
           [FromQuery] int? categoryId = null,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        // Get paginated products
        var products = await Query.GetProviderPaginatedProducts(connection, providerId, language, pageNumber, pageSize, categoryId);

        // Get total count of products for pagination
        var totalCount = await Query.GetCountProviderPaginatedProducts(connection, providerId, language, categoryId);

        // Create a response object containing products and total count
        var response = new
        {
            TotalCount = totalCount,
            Products = products
        };

        return Success(response); // Return success response with products and count
    }
    #endregion



    #region delivery
    [HttpGet("delivery-statistics")]
    public async Task<IActionResult> GetDeliveryStatistics()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        // Get delivery statistics
        var (activeDeliveryCount, deliveryType1Sum, deliveryType2Sum) = await Stats.GetDeliveryStatisticsAsync(connection);

        // Create a response object containing the statistics
        var response = new
        {
            ActiveDeliveryCount = activeDeliveryCount,
            DeliveryType1Sum = deliveryType1Sum,
            DeliveryType2Sum = deliveryType2Sum
        };

        return Ok(response); // Return success response with statistics
    }





    [HttpGet("deliveries/paginated")]
    public async Task<IActionResult> GetPaginatedDeliveriesAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedDeliveriesAsync(connection, page, pageSize);

        return Ok(new
        {
            result.TotalCount,
            providers = result.Data.ToList()
        }); // Return a successful response with the paginated data
    }
    [HttpGet("deliveries/paginated-by-term")]
    public async Task<IActionResult> GetPaginatedDeliveriesAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, string? searchTerm =  null)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedDeliveriesFilteredAsync(connection, page, pageSize, searchTerm);

        return Ok(new
        {
            result.TotalCount,
            providers = result.Data.ToList()
        }); // Return a successful response with the paginated data
    }
    #endregion

}


public class AdminInsertProviderModel
{
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public byte Language { get; set; }
    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalApprovalId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string OwnerName { get; set; } = null!;
    public string OwnerNationalId { get; set; } = null!;
    public IFormFile? OwnershipDocumentFile { get; set; }
    public IFormFile? OwnerNationalApprovalFile { get; set; }
    public string LocationName { get; set; } = null!;
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public string BuildingType { get; set; } = null!;
    public string BuildingNumber { get; set; } = null!;
    public string FloorNumber { get; set; } = null!;
    public string ApartmentNumber { get; set; } = null!;
    public string StreetNumber { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}