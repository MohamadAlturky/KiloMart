using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Delivery.Activity;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.ProductRequests.Add;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
using KiloMart.Presentation.Controllers.Domains.Drivers;
using KiloMart.Presentation.Models.Commands.ProductRequests;
using KiloMart.Presentation.Services;
using KiloMart.Requests.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KiloMart.Presentation.Controllers.Domains.Providers;

[ApiController]
[Route("api/provider")]
public class ProviderActivitiesContoller : AppController
{
    public ProviderActivitiesContoller(IDbFactory dbFactory,
     IUserContext userContext,
     IWebHostEnvironment environment)
     : base(dbFactory, userContext)
    {
        _environment = environment;
    }
    private readonly IWebHostEnvironment _environment;

    #region product request

    [HttpPost("provider/product-request/add")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Insert([FromForm] ProductRequestInsertWithFileModel request)
    {
        var (success, errors) = request.Validate();
        if (!success)
            return ValidationError(errors);
        if (request.ImageFile is null)
        {
            return ValidationError(new List<string> { "File is required" });
        }
        Guid fileName = Guid.NewGuid();
        var filePath = await FileService.SaveImageFileAsync(request.ImageFile,
            _environment.WebRootPath,
            fileName);


        var result = await ProductRequestService.Insert(_dbFactory, _userContext.Get(), new ProductRequestInsertModel()
        {
            Date = DateTime.Now,
            Description = request.Description,
            ImageUrl = filePath,
            Language = request.Language,
            MeasurementUnit = request.MeasurementUnit,
            Name = request.Name,
            OffPercentage = request.OffPercentage,
            Price = request.Price,
            ProductCategory = request.ProductCategory,
            Quantity = request.Quantity
        });
        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    #endregion

    #region my offers

    // [HttpGet("products/my-offers/paginated")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetOffersByProvider([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] byte language = 1)
    // {
    //     // Validate the page and pageSize parameters
    //     if (page < 1) page = 1;
    //     if (pageSize < 1) pageSize = 10;
    //     int providerId = _userContext.Get().Party;

    //     // Fetch the product offers and total count
    //     var (offers, totalCount) = await Query.GetProductOffersByProvider(_dbFactory, providerId, page, pageSize, language);

    //     // Return a successful response with the offers and total count
    //     return Success(new
    //     {
    //         Offers = offers,
    //         TotalCount = totalCount,
    //         PageNumber = page,
    //         PageSize = pageSize
    //     });
    // }

    // [HttpGet("products/my-offers")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetOffersByProvider([FromQuery] byte language = 1)
    // {
    //     int providerId = _userContext.Get().Party;

    //     // Fetch the product offers and total count
    //     List<ProductOfferDetails> result = await Query.GetProductOffersByProvider(_dbFactory, providerId, language);

    //     // Return a successful response with the offers and total count
    //     return Success(new
    //     {
    //         result
    //     });
    // }
    // [HttpGet("products/my-offers-by-category")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetOffersByProviderAndCategory([FromQuery] int category, [FromQuery] byte language = 1)
    // {
    //     int providerId = _userContext.Get().Party;

    //     // Fetch the product offers and total count
    //     List<ProductOfferDetails> result = await Query.GetProductOffersByProviderAndCategory(_dbFactory, providerId, language, category);

    //     // Return a successful response with the offers and total count
    //     return Success(new
    //     {
    //         result
    //     });
    // }
    #endregion

    #region orders

    [HttpGet("orders/total-done")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetTotalDone()
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        var result = await OrdersDb.GetTotalDoneOrdersAsync(
            connection,
            _userContext.Get().Party,
            (byte)OrderStatus.COMPLETED);

        return Success(new { TotalCount = result });
    }

    [HttpGet("orders/mine-by-status")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetMineByStatus([FromQuery] byte language,
    [FromQuery] byte status)
    {
        var result = await ReadOrderService.GetMineByStatusForProviderAsync(language,
            status,
            _userContext,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpGet("orders/requested-orders")]
    // [Guard([Roles.Customer])]
    public async Task<IActionResult> GetMineByStatus([FromQuery] byte language)
    {
        var result = await ReadOrderService.GetRequestedOrders(language,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpGet("orders/preparing")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetPreparingOrders([FromQuery] byte language)
    {
        var result = await ReadOrderService.GetPreparingOrders(
            language,
            _userContext.Get().Party,
            _dbFactory);

        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }

    public class AcceptOrderDto
    {
        public long OrderId { get; set; }
    }
    [HttpPost("orders/accept")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> AcceptOrder([FromBody] AcceptOrderDto acceptOrderDto)
    {
        var result = await AcceptOrderService.ProviderAccept(acceptOrderDto.OrderId, _userContext.Get(), _dbFactory);
        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpPost("orders/cancel")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Cancel([FromBody] CancelOrderRequest cancelOrderRequest)
    {
        var result = await OrderCancelService.ProviderCancel(
            cancelOrderRequest.OrderId,
            _userContext.Get(),
            _dbFactory
            );
        if (result.Success)
        {
            return Success(new
            {
                Message = "order canceled successfully",
                Order = result.Data
            });
        }
        return Fail(result.Errors);
    }
    #endregion


    #region Activities
    // [HttpGet("activities/by-date-range")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetByDateRange(
    //             [FromQuery] DateTime startDate,
    //             [FromQuery] DateTime endDate)
    // {
    //     int providerId = _userContext.Get().Party;
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var activities = await Db.GetProviderActivitiesByDateBetweenAndProviderAsync(startDate, endDate, providerId, connection);
    //     return Ok(activities);
    // }

    // [HttpGet("activities/by-date-bigger")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetByDateBigger(
    //     [FromQuery] DateTime date)
    // {
    //     int providerId = _userContext.Get().Party;
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var activities = await Db.GetProviderActivitiesByDateBiggerAndProviderAsync(date, providerId, connection);
    //     return Ok(activities);
    // }

    // [HttpGet("activities/all")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetAll()
    // {
    //     int providerId = _userContext.Get().Party;
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var activities = await Db.GetProviderActivitiesByProviderIdAsync(providerId, connection);
    //     return Ok(activities);
    // }

    [HttpGet("activities/filtered")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetActivitiesFiltered(
        [FromQuery] byte? type = null,
        [FromQuery] DateTime? startdate = null,
        [FromQuery] DateTime? enddate = null)
    {
        int providerId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetProviderActivityFilteredAsync(
            connection,
            providerId,
            type: type,
            startdate: startdate,
            enddate: enddate);

        return Success(activities);
    }

    [HttpGet("activities/balance")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetProviderWallet()
    {
        int providerId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        byte receives = (byte)DeliveryActivityType.Receives;
        byte deductions = (byte)DeliveryActivityType.Deductions;

        var wallet = await Db.GetProviderActivityTotalValueByProviderAsync(
            providerId,
            receives,
            deductions,
            connection);

        return Success(wallet);
    }

    // [HttpGet("activities/by-date-range")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetByDateRange(
    //         [FromQuery] DateTime startDate,
    //         [FromQuery] DateTime endDate)
    // {
    //     int ProviderId = _userContext.Get().Party;
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var activities = await Db.GetProviderActivitiesByDateBetweenAndProviderAsync(startDate, endDate, ProviderId, connection);
    //     return Success(activities);
    // }

    // [HttpGet("activities/by-date-bigger")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetByDateBigger(
    //     [FromQuery] DateTime date)
    // {
    //     int ProviderId = _userContext.Get().Party;
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var activities = await Db.GetProviderActivitiesByDateBiggerAndProviderAsync(date, ProviderId, connection);
    //     return Success(activities);
    // }
    // [HttpGet("activities/all")]
    // [Guard([Roles.Provider])]
    // public async Task<IActionResult> GetMine()
    // {
    //     int ProviderId = _userContext.Get().Party;
    //     using var connection = _dbFactory.CreateDbConnection();
    //     connection.Open();

    //     var activities = await Db.GetProviderActivitiesByProviderIdAsync(ProviderId, connection);
    //     return Success(activities);
    // }
    [HttpGet("sales-by-range")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetByDateRange(
       [FromQuery] int startYear,
       [FromQuery] int startMonth,
       [FromQuery] int endYear,
       [FromQuery] int endMonth)
    {
        int providerId = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var sales = await OrdersDb.GetOrderSummaryAsync(connection, providerId, startYear, startMonth, endYear, endMonth);

        return Success(sales);
    }

    [HttpGet("sales")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetByDateRange()
    {
        int providerId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var sales = await OrdersDb.GetOrderSummaryAsync(connection, providerId);
        return Success(sales);
    }

    [HttpGet("total-sales")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetTotalSales()
    {
        int providerId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var sales = await OrdersDb.GetTotalSalesAsync(connection, providerId);
        return Success(sales);
    }
    #endregion

    #region Wallets
    [HttpGet("wallet/mine")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetByProviderId()
    {
        int ProviderId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var wallet = await Db.GetProviderActivityTotalValueByProviderAsync(
            ProviderId,
            (byte)DeliveryActivityType.Receives,
            (byte)DeliveryActivityType.Deductions,
            connection);
        if (wallet == null)
        {
            return DataNotFound("No Wallet For This Provider");
        }
        return Success(wallet);
    }
    #endregion


    #region provider documents

    public class UploadProviderDocumentModel
    {
        /// <summary>
        /// Image file for the document
        /// </summary>
        public IFormFile? ImageFile { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        public byte DocumentType { get; set; }

        /// <summary>
        /// Name of the document
        /// </summary>
        public string Name { get; set; } = null!;


        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (DocumentType <= 0)
                errors.Add("Document type must be specified.");

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required.");

            if (ImageFile is null)
                errors.Add("Image file is required.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    [HttpPost("documents/upload")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Insert([FromForm] UploadProviderDocumentModel request)
    {
        (bool success, string[] errors) = request.Validate();
        if (!success)
        {
            return ValidationError(errors);
        }

        if (request.ImageFile is null)
        {
            return ValidationError(new List<string> { "File is required" });
        }

        Guid fileName = Guid.NewGuid();
        var filePath = await FileService.SaveImageFileAsync(request.ImageFile,
            _environment.WebRootPath,
            fileName);

        using var connection = _dbFactory.CreateDbConnection();

        int providerId = _userContext.Get().Party;
        int documentId = await Db.InsertProviderDocumentAsync(
            connection,
            request.Name,
            request.DocumentType,
            filePath,
            providerId);

        return Success(new { documentId });
    }

    [HttpGet("documents/mine")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetProviderDocumentsByProviderIdAsync()
    {
        int providerId = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();

        var result = await Db.GetProviderDocumentsByProviderIdAsync(providerId, connection);

        return Success(result.ToList());
    }

    [HttpGet("documents/by-id")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetProviderDocumentByIdAsync(
        [FromQuery] int id
    )
    {
        using var connection = _dbFactory.CreateDbConnection();

        ProviderDocument? providerDocument = await Db.GetProviderDocumentByIdAsync(id, connection);

        if (providerDocument is null)
        {
            return DataNotFound();
        }

        if (providerDocument.Provider != _userContext.Get().Party)
        {
            return Fail("Unauthorized: You can't access another provider's document.");
        }

        return Success(providerDocument);
    }
    #endregion


    #region offers reading
    [HttpGet("products-with-offers/browsing")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetByCategory(
           [FromQuery] byte language,
           [FromQuery] int? categoryId = null,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
    {
        int providerId = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        // Get paginated products
        var products = await Query.GetPaginatedProducts(connection, providerId, language, pageNumber, pageSize, categoryId);

        // Get total count of products for pagination
        var totalCount = await Query.GetCountPaginatedProducts(connection, providerId, language, categoryId);

        // Create a response object containing products and total count
        var response = new
        {
            TotalCount = totalCount,
            Products = products
        };

        return Success(response); // Return success response with products and count
    }

    #endregion



    #region offers reading mine
    [HttpGet("products-with-offers/mine")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetMineByCategory(
           [FromQuery] byte language,
           [FromQuery] int? categoryId = null,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
    {
        int providerId = _userContext.Get().Party;

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


    #region Withdrawal Actions

    [HttpPost("Withdraw/create")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> InsertWithdrawAsync([FromBody] InsertWithdrawRequest request)
    {
        int deliveryID = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var id = await Db.InsertWithdrawAsync(
            connection,
            deliveryID,
            request.BankAccountNumber,
            request.IbanNumber,
            DateTime.Now,
            false,
            false,
            false);

        return Success(new { Id = id });
    }

    [HttpPut("Withdraw/update/{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> UpdateWithdrawAsync(long id, [FromBody] UpdateWithdrawRequest request)
    {
        int deliveryID = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();
        Withdraw? withdraw = await Db.GetWithdrawByIdAsync(id, connection);
        if (withdraw is null)
        {
            return DataNotFound();
        }
        if (withdraw.Party != deliveryID)
        {
            return Fail("Un Authorized this withdraw isn't for you");
        }
        if (withdraw.Done)
        {
            return Fail("this withdraw is done!! can't edit it");
        }
        var success = await Db.UpdateWithdrawAsync(
            connection,
            id,
            withdraw.Party,
            request.BankAccountNumber ?? withdraw.BankAccountNumber,
            request.IbanNumber ?? withdraw.IBanNumber,
            withdraw.Date,
            withdraw.Done,
            withdraw.Accepted,
            withdraw.Rejected);

        return success ? Success() : Fail("Update failed.");
    }

    [HttpDelete("Withdraw/delete/{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> DeleteWithdrawAsync(long id)
    {
        int deliveryID = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        Withdraw? withdraw = await Db.GetWithdrawByIdAsync(id, connection);
        if (withdraw is null)
        {
            return DataNotFound();
        }
        if (withdraw.Party != deliveryID)
        {
            return Fail("Un Authorized this withdraw isn't for you");
        }
        var success = await Db.DeleteWithdrawAsync(connection, id);
        return success ? Success() : Fail("Deletion failed.");
    }

    [HttpGet("Withdraw/{id}")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetWithdrawByIdAsync(long id)
    {
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraw = await Db.GetWithdrawByIdAsync(id, connection);
        return withdraw is not null ? Success(withdraw) : DataNotFound("Withdrawal not found.");
    }
    [HttpGet("Withdraw/mine")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAsync()
    {
        int deliveryId = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByPartyAsync(deliveryId, connection);
        return Success(withdraws);
    }
    [HttpGet("Withdraw/mine/by-done")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetWithdrawsByDeliveryAsync([FromQuery] bool done)
    {
        int deliveryId = _userContext.Get().Party;

        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var withdraws = await Db.GetWithdrawsByPartyAndDoneAsync(deliveryId, done, connection);
        return Success(withdraws);
    }


    // // Request models for Insert and Update actions
    // public class InsertWithdrawRequest
    // {

    //     public string BankAccountNumber { get; set; }

    //     public string IbanNumber { get; set; }

    // }

    // public class UpdateWithdrawRequest
    // {

    //     public string? BankAccountNumber { get; set; }

    //     public string? IbanNumber { get; set; }

    // }

    #endregion

}