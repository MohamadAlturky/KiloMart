using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DeliveryProfileHistory
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string NationalName { get; set; } = null!;

    public string NationalId { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public DateTime LicenseExpiredDate { get; set; }

    public string DrivingLicenseNumber { get; set; } = null!;

    public DateTime DrivingLicenseExpiredDate { get; set; }

    public string VehicleNumber { get; set; } = null!;

    public string VehicleModel { get; set; } = null!;

    public string VehicleType { get; set; } = null!;

    public string VehicleYear { get; set; } = null!;

    public string VehiclePhotoFileUrl { get; set; } = null!;

    public string DrivingLicenseFileUrl { get; set; } = null!;

    public string VehicleLicenseFileUrl { get; set; } = null!;

    public string NationalIqamaIdfileUrl { get; set; } = null!;

    public DateTime SubmitDate { get; set; }

    public DateTime? ReviewDate { get; set; }

    public int DeliveryId { get; set; }

    public bool IsActive { get; set; }

    public bool IsRejected { get; set; }

    public bool IsAccepted { get; set; }

    public string? ReviewDescription { get; set; }
}
