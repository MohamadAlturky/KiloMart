using KiloMart.Core.Authentication;
using KiloMart.Core.Contracts;
using KiloMart.Core.Models;
using KiloMart.DataAccess.Database;
using System.Threading.Tasks;

namespace KiloMart.Commands.Services
{
    public class LocationDetailsInsertModel
    {
        public string BuildingType { get; set; } = null!;
        public string BuildingNumber { get; set; } = null!;
        public string FloorNumber { get; set; } = null!;
        public string ApartmentNumber { get; set; } = null!;
        public string StreetNumber { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int Location { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(BuildingType))
                errors.Add("Building type is required.");

            if (string.IsNullOrWhiteSpace(BuildingNumber))
                errors.Add("Building number is required.");

            if (string.IsNullOrWhiteSpace(FloorNumber))
                errors.Add("Floor number is required.");

            if (string.IsNullOrWhiteSpace(ApartmentNumber))
                errors.Add("Apartment number is required.");

            if (string.IsNullOrWhiteSpace(StreetNumber))
                errors.Add("Street number is required.");

            if (string.IsNullOrWhiteSpace(PhoneNumber))
                errors.Add("Phone number is required.");

            if (Location <= 0)
                errors.Add("Location must be a positive number.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    public class LocationDetailsUpdateModel
    {
        public int Id { get; set; }
        public string? BuildingType { get; set; }
        public string? BuildingNumber { get; set; }
        public string? FloorNumber { get; set; }
        public string? ApartmentNumber { get; set; }
        public string? StreetNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Location { get; set; }

        public (bool Success, string[] Errors) Validate()
        {
            var errors = new List<string>();

            if (Id <= 0)
                errors.Add("Location details ID must be a positive number.");

            return (errors.Count == 0, errors.ToArray());
        }
    }

    public static class LocationDetailsService
    {
        public static async Task<Result<LocationDetails>> Insert(
            IDbFactory dbFactory,
            UserPayLoad userPayLoad,
            LocationDetailsInsertModel model)
        {
            var (success, errors) = model.Validate();
            if (!success)
            {
                return Result<LocationDetails>.Fail(errors);
            }

            try
            {
                var connection = dbFactory.CreateDbConnection();
                connection.Open();
                Location? location = await Db.GetLocationByIdAsync(model.Location, connection);
                
                if (location is null)
                {
                    return Result<LocationDetails>.Fail(["Not Found"]);
                }
                if (location.Party != userPayLoad.Party)
                {
                    return Result<LocationDetails>.Fail(["Un Authorized"]);
                }
                
                var id = await Db.InsertLocationDetailsAsync(connection, model.BuildingType, model.BuildingNumber, model.FloorNumber, model.ApartmentNumber, model.StreetNumber, model.PhoneNumber, model.Location);
                var locationDetails = new LocationDetails
                {
                    Id = id,
                    BuildingType = model.BuildingType,
                    BuildingNumber = model.BuildingNumber,
                    FloorNumber = model.FloorNumber,
                    ApartmentNumber = model.ApartmentNumber,
                    StreetNumber = model.StreetNumber,
                    PhoneNumber = model.PhoneNumber,
                    Location = model.Location
                };

                return Result<LocationDetails>.Ok(locationDetails);
            }
            catch (Exception e)
            {
                return Result<LocationDetails>.Fail([e.Message]);
            }
        }

        public static async Task<Result<LocationDetails>> Update(
            IDbFactory dbFactory,
            UserPayLoad userPayLoad,
            LocationDetailsUpdateModel model)
        {
            var (success, errors) = model.Validate();
            if (!success)
            {
                return Result<LocationDetails>.Fail(errors);
            }

            try
            {
                var connection = dbFactory.CreateDbConnection();
                connection.Open();
                var existingModel = await Db.GetLocationDetailsByIdAsync(model.Id, connection);

                if (existingModel is null)
                {
                    return Result<LocationDetails>.Fail(["Not Found"]);
                }

                existingModel.BuildingType = model.BuildingType ?? existingModel.BuildingType;
                existingModel.BuildingNumber = model.BuildingNumber ?? existingModel.BuildingNumber;
                existingModel.FloorNumber = model.FloorNumber ?? existingModel.FloorNumber;
                existingModel.ApartmentNumber = model.ApartmentNumber ?? existingModel.ApartmentNumber;
                existingModel.StreetNumber = model.StreetNumber ?? existingModel.StreetNumber;
                existingModel.PhoneNumber = model.PhoneNumber ?? existingModel.PhoneNumber;
                existingModel.Location = model.Location ?? existingModel.Location;

                await Db.UpdateLocationDetailsAsync(connection, existingModel.Id, existingModel.BuildingType, existingModel.BuildingNumber, existingModel.FloorNumber, existingModel.ApartmentNumber, existingModel.StreetNumber, existingModel.PhoneNumber, existingModel.Location);

                return Result<LocationDetails>.Ok(existingModel);
            }
            catch (Exception e)
            {
                return Result<LocationDetails>.Fail([e.Message]);
            }
        }
    }
}
