using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Requests.Queries;

namespace KiloMart.Domain.OrderRequests;

public class OrderRequestService
{
    public static async Task<Result<CreateOrderRequestApiResponseModel>> Insert(
        CreateOrderRequestApiRequestModel model,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<CreateOrderRequestApiResponseModel>.Fail(errors);
        }
        var response = new CreateOrderRequestApiResponseModel();

        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();
        var location = await Db.GetLocationByIdAsync(model.LocationId, readConnection);
        if(location is null)
        {
            return Result<CreateOrderRequestApiResponseModel>.Fail(["Location Not Found"]);
        }
        if (location.Party != userPayLoad.Party)
        {
            return Result<CreateOrderRequestApiResponseModel>.Fail(["Location is not for this customer"]);
        }

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();


        
        try
        {
            response.OrderRequest = new()
            {
                Customer = userPayLoad.Party,
                OrderRequestStatus = (byte)OrderRequestStatus.Init,
                Date = DateTime.Now
            };
            response.OrderRequest.Id = await Db.InsertOrderAsync(connection,
                response.OrderRequest.Customer,
                response.OrderRequest.Date,
                response.OrderRequest.OrderRequestStatus,
                transaction);
            
            foreach (var item in model.RequestedProducts)
            {
                OrderRequestItem orderRequestItem = new()
                {
                    OrderRequest = response.OrderRequest.Id,
                    Product = item.ProductId,
                    Quantity = item.RequestedQuantity
                };
                orderRequestItem.Id = await Db.InsertOrderRequestItemAsync(connection, 
                    orderRequestItem.Product, 
                    orderRequestItem.Quantity, 
                    orderRequestItem.OrderRequest,
                    transaction);

                response.Items.Add(orderRequestItem);
            }
            var productOfferCounts = await Query.GetProductOfferCounts(readConnection, 
                model.RequestedProducts, 
                location.Latitude, 
                location.Longitude);

            response.ProductOfferCounts = productOfferCounts;
            transaction.Commit();
            return Result<CreateOrderRequestApiResponseModel>.Ok(response);
        }
        catch(Exception ex)
        {
            transaction.Rollback();
            return Result<CreateOrderRequestApiResponseModel>.Fail([ex.Message]);
        }
    }
}

public enum OrderRequestStatus
{
    Init = 1,
    Canceled = 2,
    Accepted = 3
}

public class CreateOrderRequestApiRequestModel
{
    public List<RequestedProduct> RequestedProducts { get; set; }
    public int LocationId { get; set; }
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (RequestedProducts is null)
        {
            errors.Add("No Requested Products");
        }
        if (LocationId == 0)
        {
            errors.Add("LocationId required");
        }

        if (RequestedProducts is not null && RequestedProducts.Count==0)
        {
            errors.Add("Requested Products Is Empty");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}

public class CreateOrderRequestApiResponseModel
{
    public OrderRequest OrderRequest { get; set; } = new();
    public List<OrderRequestItem> Items { get; set; } = [];
    public ProductOfferCount[] ProductOfferCounts { get; set; } = [];
}