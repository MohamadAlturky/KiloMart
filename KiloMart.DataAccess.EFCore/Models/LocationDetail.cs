using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class LocationDetail
{
    public int Id { get; set; }

    public string BuildingType { get; set; } = null!;

    public string BuildingNumber { get; set; } = null!;

    public string FloorNumber { get; set; } = null!;

    public string ApartmentNumber { get; set; } = null!;

    public string StreetNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int Location { get; set; }

    public virtual Location LocationNavigation { get; set; } = null!;
}
