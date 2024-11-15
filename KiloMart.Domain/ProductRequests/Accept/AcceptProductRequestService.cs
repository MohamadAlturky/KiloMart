using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using KiloMart.Domain.ProductRequests.Add;

namespace KiloMart.Domain.ProductRequests.Accept;

public static class AcceptProductRequestService
{
    public static async Task<Result<bool>> Accept(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        int id)
    {
        var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            ProductRequest? productRequest = await Db.GetProductRequestByIdAsync(id, connection);
            
            if (productRequest is null)
            {
                return Result<bool>.Fail(["Not Found"]);
            }
            
            if (productRequest.Status != (byte)ProductRequestStatus.Pending)
            {
                return Result<bool>.Fail(["Can't Accept a request of type not Pending"]);
            }
            List<ProductRequestDataLocalized> localizations = 
                await Db.ListProductRequestDataLocalizedAsync(productRequest.Id, connection);
            
            if(localizations is null || localizations.Count == 0)
            {
                return Result<bool>.Fail(["the localized data Not Found"]);
            }
            ProductRequestDataLocalized defaultLocalization = localizations[0];


            // insert a product
            int productId = await Db.InsertProductAsync(connection,
                productRequest.ImageUrl,
                productRequest.ProductCategory,
                true,
                defaultLocalization.MeasurementUnit,
                defaultLocalization.Description,
                defaultLocalization.Name,
                transaction);

            // insert the localizations 
            foreach (var localization in localizations)
            {
               await Db.InsertProductLocalizedAsync(connection,
                    localization.Language,
                    productId,
                    localization.MeasurementUnit,
                    localization.Description,
                    localization.Name
                    ,transaction);
            }

            // adding the provider offer
            await Db.InsertProductOfferAsync(connection,
                productId,
                productRequest.Price,
                productRequest.OffPercentage,
                DateTime.UtcNow,
                null,
                productRequest.Quantity,
                productRequest.Provider,
                transaction);

            // update the status

            await Db.UpdateProductRequestAsync(connection,
                productRequest.Id,
                productRequest.Provider,
                productRequest.Date,
                productRequest.ImageUrl,
                productRequest.ProductCategory,
                productRequest.Price,
                productRequest.OffPercentage,
                productRequest.Quantity,
                (byte)ProductRequestStatus.Accepted,
                transaction);

            transaction.Commit();
            return Result<bool>.Ok(true);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<bool>.Fail([e.Message]);
        }
    }
}
