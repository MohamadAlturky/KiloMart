using System.Data;
using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Admin;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.DateServices;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.Register.Provider.Services;
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

    public AdminPanelController(DriversTrackerService driversTrackerService, IDbFactory dbFactory,
    IUserContext userContext, IWebHostEnvironment environment) : base(dbFactory, userContext)
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


    [HttpGet("payment-yearly-stats-by-year")]
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
        // int[] CardPayments = PaymentGetDaysInMonth(year, month, stats1);
        decimal[] CardPayments = PaymentGetMonthlyOrderCounts(orderThatPaidByElcetronicStats);
        decimal[] CashPayments = PaymentGetMonthlyOrderCounts(orderThatPaidByCashStats);


        return Success(new
        {
            CardPayments,
            CashPayments
        });

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
    [HttpGet("payment-yearly-stats-by-month")]
    public async Task<IActionResult> GetOrderSummaryMonthlyAsync(
        [FromQuery] int year,
        [FromQuery] int month
    )
    {

        using var connection = _dbFactory.CreateDbConnection();

        var orderThatPaidByCashStats = await Stats.GetOrderSummaryDailyAsync(
            connection,
            true,
            (byte)PaymentType.Cash,
            year,
            month);

        var orderThatPaidByElcetronicStats = await Stats.GetOrderSummaryDailyAsync(
            connection,
            true,
            (byte)PaymentType.Elcetronic,
            year,
            month);

        decimal[] CardPayments = PaymentGetDaysInMonth(year, month, orderThatPaidByElcetronicStats);
        decimal[] CashPayments = PaymentGetDaysInMonth(year, month, orderThatPaidByCashStats);


        return Success(new
        {
            CardPayments,
            CashPayments
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
    public static int[] GetDaysInMonth(int year, int month, List<OrderCountMonthlySummary> orderCounts)
    {
        // Validate the input month
        if (month < 1 || month > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        }

        // Create a list to hold the days of the specified month

        // Get the number of days in the specified month and year
        int daysInSpecifiedMonth = DateTime.DaysInMonth(year, month);
        int[] daysInMonth = new int[daysInSpecifiedMonth];

        // Populate the list with each day of the month
        foreach (var order in orderCounts)
        {
            if (
                order.OrderMonth >= 1 && order.OrderMonth <= 12
            )
            {
                daysInMonth[order.OrderDay - 1] += order.TotalCount;
            }
        }

        return daysInMonth;
    }
    public static int[] GetMonthlyOrderCounts(List<OrderCountMonthlySummary> orderCounts)
    {
        // Initialize an array with 12 elements (for each month)
        int[] monthlyCounts = new int[12];

        // Populate the array with counts from the orderCounts list
        foreach (var order in orderCounts)
        {
            if (order.OrderMonth >= 1 && order.OrderMonth <= 12)
            {
                monthlyCounts[order.OrderMonth - 1] += order.TotalCount;
            }
        }

        return monthlyCounts;
    }

    [HttpGet("orders-count-yearly-stats-summary")]
    public async Task<IActionResult> GetOrderCountSummaryAsync(
        [FromQuery] int year
    )
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetching order statistics for the specified year and month
        //var orderStats = await GetOrderStatisticsAsync(connection, year, month);
        var stats1 = await Stats.GetOrderCountSummaryAsync(
            connection,
            true,
            ((byte)PaymentType.Elcetronic),
            year,
            (byte)OrderStatus.COMPLETED
        );

        var stats2 = await Stats.GetOrderCountSummaryAsync(
            connection,
            true,
            ((byte)PaymentType.Cash),
            year,
            (byte)OrderStatus.COMPLETED
        );
        int[] CardPayments = GetMonthlyOrderCounts(stats1);
        int[] CashPayments = GetMonthlyOrderCounts(stats2);


        return Success(new
        {
            CardPayments,
            CashPayments
        });
    }

    public static decimal[] PaymentGetMonthlyOrderCounts(List<OrderYearlySummary> orderCounts)
    {
        // Initialize an array with 12 elements (for each month)
        decimal[] monthlyCounts = new decimal[12];

        // Populate the array with counts from the orderCounts list
        foreach (var order in orderCounts)
        {
            if (order.OrderYear > 0 && order.OrderMonth >= 1 && order.OrderMonth <= 12)
            {
                monthlyCounts[order.OrderMonth - 1] += order.TotalPriceSum;
            }
        }

        return monthlyCounts;
    }

    public static decimal[] PaymentGetDaysInMonth(int year, int month, List<OrderDailySummary> dailySummaries)
    {
        // Validate the input month
        if (month < 1 || month > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        }

        // Get the number of days in the specified month and year
        int daysInSpecifiedMonth = DateTime.DaysInMonth(year, month);
        decimal[] daysInMonth = new decimal[daysInSpecifiedMonth];

        // Populate the array with each day's total count from daily summaries
        foreach (var order in dailySummaries)
        {
            if (order.OrderYear == year && order.OrderMonth == month && order.OrderDay >= 1 && order.OrderDay <= daysInSpecifiedMonth)
            {
                daysInMonth[order.OrderDay - 1] += order.TotalPriceSum;
            }
        }

        return daysInMonth;
    }

    [HttpGet("orders-count-monthly-stats-summary")]
    public async Task<IActionResult> GetOrderMonthlyCountSummaryAsync(
            [FromQuery] int year,
            [FromQuery] int month
        )
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetching order statistics for the specified year and month
        //var orderStats = await GetOrderStatisticsAsync(connection, year, month);
        var stats1 = await Stats.GetOrderCountSummaryAsync(
            connection,
            true,
            ((byte)PaymentType.Elcetronic),
            year,
            month,
            (byte)OrderStatus.COMPLETED
        );

        var stats2 = await Stats.GetOrderCountSummaryAsync(
            connection,
            true,
            ((byte)PaymentType.Cash),
            year,
            month,
            (byte)OrderStatus.COMPLETED
        );
        int[] CardPayments = GetDaysInMonth(year, month, stats1);
        int[] CashPayments = GetDaysInMonth(year, month, stats2);


        return Success(new
        {
            CardPayments,
            CashPayments
        });
    }

    // [HttpGet("orders-count-summary")]
    // public async Task<IActionResult> GetOrdersCountSummaryAsync()
    // {
    //     using var connection = _dbFactory.CreateDbConnection();

    //     // Fetching various order statistics
    //     var canceledOrderCount = await Stats.GetCanceledOrderCountAsync(connection);
    //     var completedOrderCount = await Stats.GetCompletedOrderCountAsync(connection);
    //     var totalOrderCount = await Stats.GetTotalOrderCountAsync(connection);
    //     var noProviderCount = await Stats.ThereIsNoProviderAcceptedItAsync(connection);
    //     var noDeliveryCount = await Stats.ThereIsNoDeliveryAcceptedItAsync(connection);

    //     // Create a summary object to hold the results
    //     var orderSummary = new
    //     {
    //         CanceledOrders = canceledOrderCount,
    //         CompletedOrders = completedOrderCount,
    //         TotalOrders = totalOrderCount,
    //         NoProviderAccepted = noProviderCount,
    //         NoDeliveryAccepted = noDeliveryCount
    //     };

    //     return Success(orderSummary);
    // }
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

        return Success(providersSummary); // Return a successful response with the summary
    }
    [HttpGet("providers/paginated")]
    public async Task<IActionResult> GetPaginatedProvidersAsync(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedProvidersAsync(connection, page, pageSize);

        return Success(new
        {
            result.TotalCount,
            providers = result.Data.Select(e =>
            {
                long TotalOrdersValue = e.TotalOrders;
                TotalOrdersValue = TotalOrdersValue != 0 ? TotalOrdersValue : 1;

                long TotalProductsValue = e.TotalProducts;
                TotalProductsValue = TotalProductsValue != 0 ? TotalProductsValue : 1;

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
                    WithdrawalBalance = e.WithdrawalBalance / (TotalProductsValue * TotalOrdersValue),
                    ReceivedBalance = e.ReceivedBalance / (TotalProductsValue * TotalOrdersValue),
                    TotalBalnace = e.ReceivedBalance / (TotalProductsValue * TotalOrdersValue),
                    availableBalance = (e.ReceivedBalance - e.WithdrawalBalance) / (TotalProductsValue * TotalOrdersValue),
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

        return Success(new
        {
            TotalCount = count,
            providers = result.Select(e =>
            {
                long TotalOrdersValue = e.TotalOrders;
                TotalOrdersValue = TotalOrdersValue != 0 ? TotalOrdersValue : 1;
                long TotalProductsValue = e.TotalProducts;
                TotalProductsValue = TotalProductsValue != 0 ? TotalProductsValue : 1;
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
                    WithdrawalBalance = e.WithdrawalBalance / (TotalProductsValue * TotalOrdersValue),
                    ReceivedBalance = e.ReceivedBalance / (TotalProductsValue * TotalOrdersValue),
                    TotalBalnace = e.ReceivedBalance / (TotalProductsValue * TotalOrdersValue),
                    availableBalance = (e.ReceivedBalance - e.WithdrawalBalance) / (TotalProductsValue * TotalOrdersValue),
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
            var now = SaudiDateTimeHelper.GetCurrentTime();
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
                true,
                transaction); // isActive

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
                IsAccepted = true,
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

            transaction.Commit();
            return Success(new { id, request });
            #endregion
        }
        catch (Exception ex)
        {
            transaction.Rollback();
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


        long TotalOrdersValue = statistics is not null ? statistics.TotalOrders : 0;
        TotalOrdersValue = TotalOrdersValue != 0 ? TotalOrdersValue : 1;

        long TotalProductsValue = statistics is not null ? statistics.TotalProducts : 0;
        TotalProductsValue = TotalProductsValue != 0 ? TotalProductsValue : 1;

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
            ReceivedBalance = (statistics is not null ? statistics.ReceivedBalance : 0) / (TotalProductsValue * TotalOrdersValue),
            WithdrawalBalance = (statistics is not null ? statistics.WithdrawalBalance : 0) / (TotalProductsValue * TotalOrdersValue),
            availableBalance = ((statistics is not null ? statistics.ReceivedBalance : 0) - (statistics is not null ? statistics.WithdrawalBalance : 0)) / (TotalProductsValue * TotalOrdersValue),
            totalBalance = (statistics is not null ? statistics.ReceivedBalance : 0) / (TotalProductsValue * TotalOrdersValue),
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
            ReceivesSum = deliveryType1Sum,
            DeductionsSum = deliveryType2Sum
        };

        return Success(response); // Return success response with statistics
    }





    [HttpGet("deliveries/paginated")]
    public async Task<IActionResult> GetPaginatedDeliveriesAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedDeliveriesAsync(connection, page, pageSize);

        return Success(new
        {
            result.TotalCount,
            deliveries = result.Data.Select(e =>
            {
                long value = e.TotalOrders;
                value = value != 0 ? value : 1;
                return new
                {
                    e.Id,
                    e.FirstName,
                    e.SecondName,
                    e.NationalName,
                    e.NationalId,
                    e.LicenseNumber,
                    e.LicenseExpiredDate,
                    e.DrivingLicenseNumber,
                    e.DrivingLicenseExpiredDate,
                    e.VehicleNumber,
                    e.VehicleModel,
                    e.VehicleType,
                    e.VehicleYear,
                    e.VehiclePhotoFileUrl,
                    e.DrivingLicenseFileUrl,
                    e.VehicleLicenseFileUrl,
                    e.NationalIqamaIDFileUrl,
                    e.SubmitDate,
                    e.ReviewDate,
                    e.DeliveryId,
                    //                    e.IsActive,
                    e.IsRejected,
                    e.IsAccepted,
                    e.ReviewDescription,
                    e.Email,
                    e.DisplayName,
                    TotalOrders = e.TotalOrders,
                    ReceivedBalance = e.ReceivedBalance / value,
                    WithdrawalBalance = e.WithdrawalBalance / value
                };
            }).ToList()
        }); // Return a successful response with the paginated data
    }
    [HttpGet("deliveries/paginated-by-term")]
    public async Task<IActionResult> GetPaginatedDeliveriesAsync(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10, string? searchTerm = null)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedDeliveriesFilteredAsync(connection, page, pageSize, searchTerm);

        return Success(new
        {
            result.TotalCount,
            deliveries = result.Data.Select(e =>
            {
                long value = e.TotalOrders;
                value = value != 0 ? value : 1;

                return new
                {
                    e.Id,
                    e.FirstName,
                    e.SecondName,
                    e.NationalName,
                    e.NationalId,
                    e.LicenseNumber,
                    e.LicenseExpiredDate,
                    e.DrivingLicenseNumber,
                    e.DrivingLicenseExpiredDate,
                    e.VehicleNumber,
                    e.VehicleModel,
                    e.VehicleType,
                    e.VehicleYear,
                    e.VehiclePhotoFileUrl,
                    e.DrivingLicenseFileUrl,
                    e.VehicleLicenseFileUrl,
                    e.NationalIqamaIDFileUrl,
                    e.SubmitDate,
                    e.ReviewDate,
                    e.DeliveryId,
                    IsActive = e.UserIsActive,
                    e.IsRejected,
                    e.IsAccepted,
                    e.ReviewDescription,
                    e.Email,
                    e.DisplayName,
                    TotalOrders = e.TotalOrders,
                    ReceivedBalance = e.ReceivedBalance / value,
                    WithdrawalBalance = e.WithdrawalBalance / value
                };
            }).ToList()
        }); // Return a successful response with the paginated data
    }

    [HttpPost("create-delivery-directly")]
    public async Task<IActionResult> InsertDelivery(AdminInsertDeliveryModel request)
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


            #region VehiclePhotoFile

            Guid fileName = Guid.NewGuid();
            if (request.VehiclePhotoFile is null)
            {
                return Fail("VehiclePhotoFile is null");
            }

            var VehiclePhotoFilePath = await FileService.SaveImageFileAsync(request.VehiclePhotoFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(VehiclePhotoFilePath))
            {
                return Fail("failed to save VehiclePhoto file");
            }
            #endregion

            #region DrivingLicenseFile

            // Save DrivingLicenseFile

            fileName = Guid.NewGuid();
            if (request.DrivingLicenseFile is null)
            {
                return Fail("DrivingLicenseFile is null");
            }
            var DrivingLicenseFilePath = await FileService.SaveImageFileAsync(request.DrivingLicenseFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(DrivingLicenseFilePath))
            {
                return Fail("failed to save DrivingLicense file");
            }
            #endregion

            #region NationalIqamaIDFile
            // Save NationalIqamaIDFile

            fileName = Guid.NewGuid();
            if (request.NationalIqamaIDFile is null)
            {
                return Fail("NationalIqamaIDFile is null");
            }
            var NationalIqamaIDFilePath = await FileService.SaveImageFileAsync(request.NationalIqamaIDFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(NationalIqamaIDFilePath))
            {
                return Fail("failed to save NationalIqamaID file");
            }

            #endregion

            #region VehicleLicenseFile

            fileName = Guid.NewGuid();
            if (request.VehicleLicenseFile is null)
            {
                return Fail("VehicleLicenseFile is null");
            }
            var VehicleLicenseFilePath = await FileService.SaveImageFileAsync(request.VehicleLicenseFile,
                _environment.WebRootPath,
                fileName);

            if (string.IsNullOrEmpty(VehicleLicenseFilePath))
            {
                return Fail("failed to save VehicleLicense file");
            }
            #endregion
            var now = SaudiDateTimeHelper.GetCurrentTime();
            #region Adding the Profile
            long id = await Db.InsertDeliveryProfileHistoryAsync(
                connection,
                request.FirstName,
                request.SecondName,
                request.NationalName,
                request.NationalId,
                request.LicenseNumber,
                request.LicenseExpiredDate,
                request.DrivingLicenseNumber,
                request.DrivingLicenseExpiredDate,
                request.VehicleNumber,
                request.VehicleModel,
                request.VehicleType,
                request.VehicleYear,
                VehiclePhotoFilePath,
                DrivingLicenseFilePath,
                VehicleLicenseFilePath,
                NationalIqamaIDFilePath,
                now,
                now,
                result.PartyId.Value,
                true,
                false,
                true,
                transaction);
            #endregion          

            #region Returning The Response
            var model = new
            {
                request.FirstName,
                request.SecondName,
                request.NationalName,
                request.NationalId,
                request.LicenseNumber,
                request.LicenseExpiredDate,
                request.DrivingLicenseNumber,
                request.DrivingLicenseExpiredDate,
                request.VehicleNumber,
                request.VehicleModel,
                request.VehicleType,
                request.VehicleYear,
                VehiclePhotoFilePath,
                DrivingLicenseFilePath,
                VehicleLicenseFilePath,
                NationalIqamaIDFilePath,
                SubmitDate = now,
                DeliveryId = result.PartyId.Value,
                IsActive = false,
                IsRejected = false,
                IsAccepted = false,

                ReviewDate = now,
                request.DisplayName,
                request.Language
            };
            transaction.Commit();
            return Success(new { id, model });
            #endregion
        }
        catch
        {
            transaction.Rollback();
            throw;
            // return Fail("Failed to insert delivery profile history.");
        }
    }
    [HttpGet("delivery-by-id")]
    public async Task<IActionResult> GetDeliveryById(
        [FromQuery] int deliveryId
    )
    {
        using var connection = _dbFactory.CreateDbConnection();
        var user = await Db.GetMembershipUserByPartyAsync(connection, deliveryId);
        var party = await Db.GetPartyByIdAsync(deliveryId, connection);
        var profile = await Db.GetDeliveryActiveProfileHistoryAsync(connection, deliveryId);
        var statistics = await Stats.GetDeliveryStatisticsAsync(connection, deliveryId);


        long TotalOrdersValue = statistics is not null ? statistics.TotalOrders : 0;
        TotalOrdersValue = TotalOrdersValue != 0 ? TotalOrdersValue : 1;


        return Success(new
        {
            displayName = party?.DisplayName,
            firstName = profile?.FirstName,
            secondName = profile?.SecondName,
            userId = user?.Id,
            email = user?.Email,
            isActive = user?.IsActive,
            isEmailVerified = user?.EmailConfirmed,
            profile?.NationalName,
            profile?.NationalId,
            profile?.LicenseNumber,
            profile?.LicenseExpiredDate,
            profile?.DrivingLicenseNumber,
            profile?.DrivingLicenseExpiredDate,
            profile?.VehicleNumber,
            profile?.VehicleModel,
            profile?.VehicleType,
            profile?.VehicleYear,
            profile?.VehiclePhotoFileUrl,
            profile?.DrivingLicenseFileUrl,
            profile?.VehicleLicenseFileUrl,
            profile?.NationalIqamaIDFileUrl,
            profile?.SubmitDate,
            profile?.ReviewDate,
            profile?.DeliveryId,
            profile?.IsRejected,
            profile?.IsAccepted,
            profile?.ReviewDescription,
            totalOrders = statistics is not null ? statistics.TotalOrders : 0,
            ReceivedBalance = (statistics is not null ? statistics.ReceivedBalance : 0) / TotalOrdersValue,
            WithdrawalBalance = (statistics is not null ? statistics.WithdrawalBalance : 0) / TotalOrdersValue,
            availableBalance = ((statistics is not null ? statistics.ReceivedBalance : 0) - (statistics is not null ? statistics.WithdrawalBalance : 0)) / TotalOrdersValue,
            totalBalance = (statistics is not null ? statistics.ReceivedBalance : 0) / TotalOrdersValue,
        });
    }
    [HttpGet("delivery-orders-by-id")]
    public async Task<IActionResult> GetDeliveryOrdersById(
       [FromQuery] int deliveryId,
       [FromQuery] byte language
       )
    {
        var result = await ReadOrderService.GetForDeliveryAsync(language, deliveryId, _dbFactory);

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
    #endregion


    #region customer

    [HttpGet("customers-summary")]
    public async Task<IActionResult> GetCustomersSummaryAsync()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        // Fetching provider statistics
        var stats = await Stats.GetCustomerMembershipCountsAsync(connection);

        // Create a summary object to hold the results
        var customerSummary = new
        {

            ActiveCustomersCount = stats.ActiveCount,
            InactiveCustomersCount = stats.InactiveCount,

        };

        return Success(customerSummary); // Return a successful response with the summary
    }


    [HttpGet("customer-by-id")]
    public async Task<IActionResult> GetCustomerById(
       [FromQuery] int customerId
   )
    {
        using var connection = _dbFactory.CreateDbConnection();
        var user = await Db.GetMembershipUserByPartyAsync(connection, customerId);
        var party = await Db.GetPartyByIdAsync(customerId, connection);
        var profile = await Db.GetCustomerProfileByCustomerIdAsync(customerId, connection);
        var statistics = await Stats.GetCustomerOrderStatisticsAsync(connection, customerId);
        var locations = await Stats.GetLocationsByPartyAsync(connection, customerId);



        return Success(new
        {
            displayName = party?.DisplayName,
            customerId,
            userId = user?.Id,
            email = user?.Email,
            isActive = user?.IsActive,
            profile?.Customer,
            profile?.FirstName,
            profile?.SecondName,
            profile?.NationalName,
            profile?.NationalId,
            isEmailVerified = user?.EmailConfirmed,
            statistics?.OrdersCount,
            statistics?.OrdersValue,
            locations
        });
    }
    [HttpGet("customers-orders-by-id")]
    public async Task<IActionResult> GetCustomersrOrdersById(
       [FromQuery] int customerId,
       [FromQuery] byte language
       )
    {
        var result = await ReadOrderService.GeteForCustomerAsync(language, customerId, _dbFactory);

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

    [HttpGet("customers/paginated")]
    public async Task<IActionResult> GetPaginatedCustomersFilteredAsync(
       [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? term = null)
    {
        using var connection = _dbFactory.CreateDbConnection();

        // Fetch paginated providers data
        var result = await Stats.GetPaginatedCustomersFilteredAsync(connection, page, pageSize, term);

        return Success(new
        {
            result.TotalCount,
            customers = result.Items.Select(x =>
            new
            {

                x.Id,
                x.Email,
                x.EmailConfirmed,
                x.Role,
                x.Party,
                x.IsActive,
                x.Language,
                x.IsDeleted,
                x.DisplayName,
                x.Balance,
            }).ToList()
        });
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


public class AdminInsertDeliveryModel
{

    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public byte Language { get; set; }

    public string FirstName { get; set; } = null!;
    public string SecondName { get; set; } = null!;
    public string NationalName { get; set; } = null!;
    public string NationalId { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateTime DrivingLicenseExpiredDate { get; set; }
    public string VehicleNumber { get; set; } = null!;
    public string VehicleModel { get; set; } = null!;
    public string VehicleType { get; set; } = null!;
    public string VehicleYear { get; set; } = null!;
    public IFormFile? VehiclePhotoFile { get; set; } = null!;
    public IFormFile? DrivingLicenseFile { get; set; } = null!;
    public IFormFile? VehicleLicenseFile { get; set; } = null!;
    public IFormFile? NationalIqamaIDFile { get; set; } = null!;
}

