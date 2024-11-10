// using Dapper;
// using KiloMart.Core.Contracts;
// using KiloMart.Core.Models;
// using KiloMart.Domain.Vehicles.Models;

// namespace KiloMart.Domain.Vehicles.Services;

// public static class VehicleService
// {
//     public static async Task<Result<CreateVehicleResponse>> Create(CreateVehicleRequest vehicle, IDbFactory dbFactory)
//     {
//         try
//         {
//             using var connection = dbFactory.CreateDbConnection();
//             connection.Open();

//             const string sql = @"
//                 INSERT INTO Vehicle (Number, Model, Type, Year, Delivery) 
//                 VALUES (@Number, @Model, @Type, @Year, @Delivery);
//                 SELECT CAST(SCOPE_IDENTITY() as int);";

//             CreateVehicleResponse response = new();
//             response.Id = await connection.QuerySingleAsync<int>(sql, new
//             {
//                 vehicle.Number,
//                 vehicle.Model,
//                 vehicle.Type,
//                 vehicle.Year,
//                 vehicle.Delivery,
//                 IsActive = true
//             });

//             response.Number = vehicle.Number;
//             response.Model = vehicle.Model;
//             response.Type = vehicle.Type;
//             response.Year = vehicle.Year;
//             response.Delivery = vehicle.Delivery;

//             return Result<CreateVehicleResponse>.Ok(response);
//         }
//         catch (Exception e)
//         {
//             return Result<CreateVehicleResponse>.Fail([e.Message]);
//         }
//     }
// }
