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
        public float Quantity { get; set; }

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

            if (FromDate.AddMinutes(1) < DateTime.UtcNow)
                errors.Add("From date must be in the future");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    public class ProductOfferUpdateModel
    {
        public int Id { get; set; }
        public decimal? OffPercentage { get; set; }
        public float? Quantity { get; set; }

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
                    null,
                    model.Quantity,
                    userPayLoad.Party);
                var productOffer = new ProductOffer
                {
                    Id = id,
                    Product = model.Product,
                    Price = model.Price,
                    OffPercentage = model.OffPercentage,
                    FromDate = model.FromDate,
                    ToDate = null,
                    Quantity = model.Quantity,
                    Provider = userPayLoad.Party,
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
                if (existingModel.Provider != userPayLoad.Party)
                {
                    return Result<ProductOffer>.Fail(["Un Authorized"]);
                }

                existingModel.OffPercentage = model.OffPercentage ?? existingModel.OffPercentage;
                existingModel.Quantity = model.Quantity ?? existingModel.Quantity;

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
        public static async Task<Result<ProductOffer>> DeActivate(
            IDbFactory dbFactory,
            UserPayLoad userPayLoad,
            int id)
        {
            

            try
            {
                var connection = dbFactory.CreateDbConnection();
                connection.Open();
                var existingModel = await Db.GetProductOfferByIdAsync(id, connection);

                if (existingModel is null)
                {
                    return Result<ProductOffer>.Fail(["Not Found"]);
                }
                if (existingModel.Provider != userPayLoad.Party)
                {
                    return Result<ProductOffer>.Fail(["Un Authorized"]);
                }

                existingModel.IsActive = false;

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
