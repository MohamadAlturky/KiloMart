namespace KiloMart.Domain.Deliveries.Profile;

public class CreateDeliveryProfileRequest
{
    public int Delivery { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public DateTime DrivingLicenseExpiredDate { get; set; }
}

public class CreateDeliveryProfileResponse
{
    public int Id { get; set; }
    public int Delivery { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public DateTime DrivingLicenseExpiredDate { get; set; }
}

public class UpdateDeliveryProfileRequest
{
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? NationalName { get; set; }
    public string? NationalId { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiredDate { get; set; }
    public string? DrivingLicenseNumber { get; set; }
    public DateTime? DrivingLicenseExpiredDate { get; set; }
}

public class UpdateDeliveryProfileResponse
{
    public int DeliveryId { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
    public string LicenseNumber { get; set; }
    public DateTime LicenseExpiredDate { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public DateTime DrivingLicenseExpiredDate { get; set; }
}
