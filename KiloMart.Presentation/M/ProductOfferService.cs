using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;

namespace KiloMart.Commands.Services
{
    public class ProductOfferInsertModel
    {
        public int Product { get; set; }
        public decimal Price { get; set; }
        public decimal OffPercentage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public float Quantity { get; set; }
        public int Provider { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (Product <= 0)
                errors.Add("Product ID must be a positive number.");

            if (Price <= 0)
                errors.Add("Price must be a positive number.");

            if (OffPercentage < 0 || OffPercentage > 100)
                errors.Add("Off percentage must be between 0 and 100.");

            if (Quantity <= 0)
                errors.Add("Quantity must be a positive number.");

            if (Provider <= 0)
                errors.Add("Provider ID must be a positive number.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    public class ProductOfferUpdateModel
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public decimal? OffPercentage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public float? Quantity { get; set; }
        public int? Provider { get; set; }
        public bool? IsActive { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (Id <= 0)
                errors.Add("Product offer ID must be a positive number.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    public static class ProductOfferService
    {
        public static async Task<Result<ProductOffer>> Insert(
            IDbFactory dbFactory,
            UserPayLoad userPayLoad,
            ProductOfferInsertModel model)
        {
            var (success, errors) = model.Validate();
            if (!success)
            {
                return Result<ProductOffer>.Fail(errors);
            }

            try
            {
                var connection = dbFactory.CreateDbConnection();
                connection.Open();
                var id = await Db.InsertProductOfferAsync(connection, 
                    model.Product, 
                    model.Price, 
                    model.OffPercentage, 
                    model.FromDate, 
                    model.ToDate, 
                    model.Quantity, 
                    model.Provider);
                var productOffer = new ProductOffer
                {
                    Id = id,
                    Product = model.Product,
                    Price = model.Price,
                    OffPercentage = model.OffPercentage,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    Quantity = model.Quantity,
                    Provider = model.Provider,
                    IsActive = true
                };

                return Result<ProductOffer>.Ok(productOffer);
            }
            catch (Exception e)
            {
                return Result<ProductOffer>.Fail([e.Message]);
            }
        }

        public static async Task<Result<ProductOffer>> Update(
            IDbFactory dbFactory,
            UserPayLoad userPayLoad,
            ProductOfferUpdateModel model)
        {
            var (success, errors) = model.Validate();
            if (!success)
            {
                return Result<ProductOffer>.Fail(errors);
            }

            try
            {
                var connection = dbFactory.CreateDbConnection();
                connection.Open();
                var existingModel = await Db.GetProductOfferByIdAsync(model.Id, connection);

                if (existingModel is null)
                {
                    return Result<ProductOffer>.Fail(["Not Found"]);
                }

                existingModel.Price = model.Price ?? existingModel.Price;
                existingModel.OffPercentage = model.OffPercentage ?? existingModel.OffPercentage;
                existingModel.FromDate = model.FromDate ?? existingModel.FromDate;
                existingModel.ToDate = model.ToDate ?? existingModel.ToDate;
                existingModel.Quantity = model.Quantity ?? existingModel.Quantity;
                existingModel.Provider = model.Provider ?? existingModel.Provider;
                existingModel.IsActive = model.IsActive ?? existingModel.IsActive;

                await Db.UpdateProductOfferAsync(connection,
                    existingModel.Id,
                    existingModel.Product,
                    existingModel.Price,
                    existingModel.OffPercentage,
                    existingModel.FromDate,
                    existingModel.ToDate,
                    existingModel.Quantity,
                    existingModel.Provider,
                    existingModel.IsActive);

                return Result<ProductOffer>.Ok(existingModel);
            }
            catch (Exception e)
            {
                return Result<ProductOffer>.Fail([e.Message]);
            }
        }
    }
}
