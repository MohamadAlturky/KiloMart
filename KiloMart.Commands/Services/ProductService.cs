// using KiloMart.Core.Authentication;
// using KiloMart.Core.Contracts;
// using KiloMart.Core.Models;
// using KiloMart.DataAccess.Database;


// public class ProductInsertModel
// {
//     public string Name { get; set; } = null!;
//     public string Description { get; set; } = null!;
//     public decimal Price { get; set; }

//     public (bool Success, string[] Errors) Validate()
//     {
//         var errors = new List<string>();

//         if (string.IsNullOrWhiteSpace(Name))
//             errors.Add("Product name is required.");

//         if (string.IsNullOrWhiteSpace(Description))
//             errors.Add("Product description is required.");

//         if (Price <= 0)
//             errors.Add("Product price must be a positive number.");

//         return (errors.Count == 0, errors.ToArray());
//     }
// }

// public class ProductUpdateModel
// {
//     public int Id { get; set; }
//     public string? Name { get; set; }
//     public string? Description { get; set; }
//     public decimal? Price { get; set; }
//     public bool? IsActive { get; set; }

//     public (bool Success, string[] Errors) Validate()
//     {
//         var errors = new List<string>();

//         if (Id <= 0)
//             errors.Add("Product ID must be a positive number.");

//         if (Name != null && string.IsNullOrWhiteSpace(Name))
//             errors.Add("Product name is required.");

//         if (Description != null && string.IsNullOrWhiteSpace(Description))
//             errors.Add("Product description is required.");

//         if (Price != null && (decimal)Price <= 0)
//             errors.Add("Product price must be a positive number.");

//         return (errors.Count == 0, errors.ToArray());
//     }
// }

// public static class ProductService
// {
//     public static async Task<Result<Product>> Insert(
//         IDbFactory dbFactory,
//         UserPayLoad userPayLoad,
//         ProductInsertModel model)
//     {
//         var (success, errors) = model.Validate();
//         if (!success)
//         {
//             return Result<Product>.Fail(errors);
//         }

//         try
//         {
//             var connection = dbFactory.CreateDbConnection();
//             connection.Open();
//             var id = await Db.InsertProductAsync(connection,
//              model.Name,
//               model.Description,
//                model.Price,);
//             var product = new Product
//             {
//                 Id = id,
//                 Name = model.Name,
//                 Description = model.Description,
//                 Price = model.Price,
//                 IsActive = true
//             };

//             return Result<Product>.Ok(product);
//         }
//         catch (Exception e)
//         {
//             return Result<Product>.Fail([e.Message]);
//         }
//     }

//     public static async Task<Result<Product>> Update(
//         IDbFactory dbFactory,
//         UserPayLoad userPayLoad,
//         ProductUpdateModel model)
//     {
//         var (success, errors) = model.Validate();
//         if (!success)
//         {
//             return Result<Product>.Fail(errors);
//         }

//         try
//         {
//             var connection = dbFactory.CreateDbConnection();
//             connection.Open();
//             var existingModel = await Db.GetProductByIdAsync(model.Id, connection);

//             if (existingModel is null)
//             {
//                 return Result<Product>.Fail(["Not Found"]);
//             }

//             existingModel.Name = model.Name ?? existingModel.Name;
//             existingModel.Description = model.Description ?? existingModel.Description;
//             existingModel.Price = model.Price ?? existingModel.Price;
//             existingModel.IsActive = model.IsActive ?? existingModel.IsActive;

//             await Db.UpdateProductAsync(connection, 
//                 existingModel.Id,
//                 existingModel.Name,
//                 existingModel.Description,
//                 existingModel.Price,
//                 existingModel.IsActive);

//             return Result<Product>.Ok(existingModel);
//         }
//         catch (Exception e)
//         {
//             return Result<Product>.Fail([e.Message]);
//         }
//     }
// }
