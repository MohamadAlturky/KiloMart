using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.Orders.Common;
using KiloMart.Domain.Orders.DataAccess;
using KiloMart.Requests.Queries;

namespace KiloMart.Domain.Orders.Services;

public class RequestOrderService
{
    public static async Task<Result<CreateOrderResponseModel>> Insert(
        CreateOrderRequestModel model,
        UserPayLoad userPayLoad,
        IDbFactory dbFactory)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<CreateOrderResponseModel>.Fail(errors);
        }
        var response = new CreateOrderResponseModel();

        using var readConnection = dbFactory.CreateDbConnection();
        readConnection.Open();
        var location = await Db.GetLocationByIdAsync(model.LocationId, readConnection);
        if (location is null)
        {
            return Result<CreateOrderResponseModel>.Fail(["Location Not Found"]);
        }
        if (location.Party != userPayLoad.Party)
        {
            return Result<CreateOrderResponseModel>.Fail(["Location is not for this customer"]);
        }

        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();



        try
        {
            response.Order = new()
            {
                OrderStatus = (byte)OrderStatus.ORDER_PLACED,
                TotalPrice = 0,
                TransactionId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                PaymentType = model.PaymentType,
                IsPaid = false
            };

            response.Order.Id = await OrdersDb.InsertOrderAsync(connection,
                response.Order.OrderStatus,
                response.Order.TotalPrice,
                response.Order.TransactionId,
                response.Order.Date,
                response.Order.IsPaid,
                response.Order.PaymentType,
                transaction);

            foreach (var code in model.DiscountCodes)
            {
                var discountCode = await Db.GetDiscountCodeByCodeAsync(code, readConnection);
                if (discountCode is not null)
                {
                    if (discountCode.IsActive)
                    {
                        await Db.CreateOrderDiscountCodeAsync(connection,
                                        response.Order.Id,
                                        discountCode.Id,
                                        transaction);
                        response.DiscountCodes.Add(discountCode);
                    }
                }
            }

            response.CustomerInformation = new()
            {
                Order = response.Order.Id,
                Customer = userPayLoad.Party,
                Location = model.LocationId
            };

            response.CustomerInformation.Id = await OrdersDb.InsertOrderCustomerInfoAsync(connection,
                response.CustomerInformation.Order,
                response.CustomerInformation.Customer,
                response.CustomerInformation.Location,
                transaction);

            OrderActivity activity = new()
            {
                Date = DateTime.Now,
                Order = response.Order.Id,
                OperatedBy = userPayLoad.Party,
                OrderActivityType = (byte)OrderActivityType.InitByCustomer
            };

            await OrdersDb.InsertOrderActivityAsync(connection,
                activity.Order,
                activity.Date,
                activity.OrderActivityType,
                activity.OperatedBy,
                transaction);

            foreach (var item in model.RequestedProducts)
            {
                OrderProduct orderItem = new()
                {
                    Order = response.Order.Id,
                    Product = item.ProductId,
                    Quantity = item.RequestedQuantity
                };
                orderItem.Id = await OrdersDb.InsertOrderProductAsync(connection,
                    orderItem.Order,
                    orderItem.Product,
                    orderItem.Quantity,
                    transaction);

                response.Items.Add(orderItem);
            }
            // var productOfferCounts = await Query.GetProductOfferCounts(readConnection,
            //     model.RequestedProducts,
            //     location.Latitude,
            //     location.Longitude);

            // response.ProductOfferCounts = productOfferCounts;
            transaction.Commit();
            return Result<CreateOrderResponseModel>.Ok(response);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<CreateOrderResponseModel>.Fail([ex.Message]);
        }
    }
}

public class CreateOrderRequestModel
{
    public List<RequestedProduct> RequestedProducts { get; set; } = null!;
    public List<string> DiscountCodes { get; set; } = null!;
    public int LocationId { get; set; }
    public byte PaymentType { get; set; }
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (RequestedProducts is null)
        {
            errors.Add("No Requested Products");
        }
        if (DiscountCodes is null)
        {
            errors.Add("DiscountCodes shouldn't be null, you can send it as an empty array");
        }
        if (LocationId == 0)
        {
            errors.Add("LocationId required");
        }
        if (PaymentType == 0)
        {
            errors.Add("PaymentType required");
        }

        if (RequestedProducts is not null && RequestedProducts.Count == 0)
        {
            errors.Add("Requested Products Is Empty");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}

public class CreateOrderResponseModel
{
    public Order Order { get; set; } = new();
    public List<OrderProduct> Items { get; set; } = [];
    public List<KiloMart.DataAccess.Database.DiscountCode> DiscountCodes { get; set; } = [];
    public OrderCustomerInformation CustomerInformation { get; set; } = new();
    //public ProductOfferCount[] ProductOfferCounts { get; set; } = [];
}


