using Dapper;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.Domain.Products.Offers.Models;

namespace KiloMart.Domain.Products.Offers.Services;

public static class ProductOfferService
{
    public static Result<ProductOfferDto> Insert(IDbFactory dbFactory, ProductOfferDto offer)
    {
        using var connection = dbFactory.CreateDbConnection();
        connection.Open();
        try
        {
            const string sql = @"
                INSERT INTO ProductOffer (Product, Price, OffPercentage, FromDate, ToDate, Quantity, Provider, IsActive)
                VALUES (@Product, @Price, @OffPercentage, @FromDate, @ToDate, @Quantity, @Provider, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            offer.Id = connection.QuerySingle<int>(sql,
                new
                {
                    Product = offer.Product,
                    Price = offer.Price,
                    OffPercentage = offer.OffPercentage,
                    FromDate = offer.FromDate,
                    ToDate = offer.ToDate,
                    Quantity = offer.Quantity,
                    Provider = offer.Provider,
                    IsActive = offer.IsActive
                });

            return Result<ProductOfferDto>.Ok(offer);
        }
        catch (Exception e)
        {
            return Result<ProductOfferDto>.Fail([e.Message]);
        }
    }
}
