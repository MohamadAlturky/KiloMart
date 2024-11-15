using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Domain.ProductRequests;


public class ProductRequestInsertModel
{
    public DateTime Date { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int ProductCategory { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public float Quantity { get; set; }

    /// <summary>
    /// Data
    /// </summary>
    public byte Language { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string MeasurementUnit { get; set; } = null!;


    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();



        if (string.IsNullOrWhiteSpace(ImageUrl))
            errors.Add("Image URL is required.");

        if (ProductCategory <= 0)
            errors.Add("Product category must be a positive integer.");

        if (Price <= 0)
            errors.Add("Price must be greater than zero.");

        if (OffPercentage < 0 || OffPercentage > 100)
            errors.Add("Off percentage must be between 0 and 100.");

        if (Quantity <= 0)
            errors.Add("Quantity must be greater than zero.");

        if (Language < 1)
            errors.Add("Language must be specified.");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Description is required.");

        if (string.IsNullOrWhiteSpace(MeasurementUnit))
            errors.Add("Measurement unit is required.");


        return (errors.Count == 0, errors.ToArray());
    }
}

public static class ProductRequestService
{
    public static async Task<Result<InsertProductRequestResponse>> Insert(
        IDbFactory dbFactory,
        UserPayLoad userPayLoad,
        ProductRequestInsertModel model)
    {
        var (success, errors) = model.Validate();
        if (!success)
        {
            return Result<InsertProductRequestResponse>.Fail(errors);
        }
        var connection = dbFactory.CreateDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {

            var id = await Db.InsertProductRequestAsync(connection,
                userPayLoad.Party,
                model.Date,
                model.ImageUrl,
                model.ProductCategory,
                model.Price,
                model.OffPercentage,
                model.Quantity,
                (byte)ProductRequestStatus.Pending,
                transaction);

            var productRequest = new ProductRequest
            {
                Id = id,
                Provider = userPayLoad.Party,
                Date = model.Date,
                ImageUrl = model.ImageUrl,
                ProductCategory = model.ProductCategory,
                Price = model.Price,
                OffPercentage = model.OffPercentage,
                Quantity = model.Quantity,
                Status = (byte)ProductRequestStatus.Pending
            };

            // data
            await Db.InsertProductRequestDataLocalizedAsync(connection,
                id,
                model.Language,
                model.Name,
                model.Description,
                model.MeasurementUnit,
                transaction);

            var localizedData = new ProductRequestDataLocalized
            {
                ProductRequest = id,
                Language = model.Language,
                Name = model.Name,
                Description = model.Description,
                MeasurementUnit = model.MeasurementUnit
            };


            transaction.Commit();
            return Result<InsertProductRequestResponse>.Ok(new()
            {
                ProductRequestDataLocalized = localizedData,
                ProductRequest = productRequest
            });
        }
        catch (Exception e)
        {
            transaction.Rollback();
            return Result<InsertProductRequestResponse>.Fail([e.Message]);
        }
    }
}
public class InsertProductRequestResponse
{
    public ProductRequest ProductRequest { get; set; }
    public ProductRequestDataLocalized ProductRequestDataLocalized { get; set; }
}