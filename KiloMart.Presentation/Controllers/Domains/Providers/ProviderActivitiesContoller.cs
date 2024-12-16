using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Services;
using KiloMart.Domain.ProductRequests.Add;
using KiloMart.Domain.Register.Utils;
using KiloMart.Presentation.Authorization;
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

    [HttpGet("products/my-offers/paginated")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetOffersByProvider([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] byte language = 1)
    {
        // Validate the page and pageSize parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        int providerId = _userContext.Get().Party;

        // Fetch the product offers and total count
        var (offers, totalCount) = await Query.GetProductOffersByProvider(_dbFactory, providerId, page, pageSize, language);

        // Return a successful response with the offers and total count
        return Success(new
        {
            Offers = offers,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    [HttpGet("products/my-offers")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetOffersByProvider([FromQuery] byte language = 1)
    {
        int providerId = _userContext.Get().Party;

        // Fetch the product offers and total count
        List<ProductOfferDetails> result = await Query.GetProductOffersByProvider(_dbFactory, providerId, language);

        // Return a successful response with the offers and total count
        return Success(new
        {
            result
        });
    }
    [HttpGet("products/my-offers-by-category")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetOffersByProviderAndCategory([FromQuery] int category, [FromQuery] byte language = 1)
    {
        int providerId = _userContext.Get().Party;

        // Fetch the product offers and total count
        List<ProductOfferDetails> result = await Query.GetProductOffersByProviderAndCategory(_dbFactory, providerId, language, category);

        // Return a successful response with the offers and total count
        return Success(new
        {
            result
        });
    }
    #endregion

    #region orders
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
    [HttpPost("orders/accept")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> AcceptOrder([FromBody] long orderId)
    {
        var result = await AcceptOrderService.ProviderAccept(orderId, _userContext.Get(), _dbFactory);
        return result.Success ? Success(result.Data) : Fail(result.Errors);
    }
    [HttpPost("orders/cancel")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> Cancel([FromBody] long orderId)
    {
        var result = await OrderCancelService.ProviderCancel(
            orderId,
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

    [HttpGet("activities/by-date-range")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
    {
        int ProviderId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetProviderActivitiesByDateBetweenAndProviderAsync(startDate, endDate, ProviderId, connection);
        return Success(activities);
    }

    [HttpGet("activities/by-date-bigger")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetByDateBigger(
        [FromQuery] DateTime date)
    {
        int ProviderId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetProviderActivitiesByDateBiggerAndProviderAsync(date, ProviderId, connection);
        return Success(activities);
    }
    [HttpGet("activities/all")]
    [Guard([Roles.Provider])]
    public async Task<IActionResult> GetMine()
    {
        int ProviderId = _userContext.Get().Party;
        using var connection = _dbFactory.CreateDbConnection();
        connection.Open();

        var activities = await Db.GetProviderActivitiesByProviderIdAsync(ProviderId, connection);
        return Success(activities);
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

        var wallet = await Db.GetProviderWalletByProviderIdAsync(ProviderId, connection);
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
}