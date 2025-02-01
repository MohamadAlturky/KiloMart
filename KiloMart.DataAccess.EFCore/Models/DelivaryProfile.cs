using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DelivaryProfile
{
    public int Id { get; set; }

    public int Delivary { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string NationalName { get; set; } = null!;

    public string NationalId { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public DateOnly LicenseExpiredDate { get; set; }

    public string DrivingLicenseNumber { get; set; } = null!;

    public DateOnly DrivingLicenseExpiredDate { get; set; }

    public virtual Delivery DelivaryNavigation { get; set; } = null!;
}
