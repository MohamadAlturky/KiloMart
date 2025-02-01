using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProviderProfileHistory
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string NationalApprovalId { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public string OwnerNationalId { get; set; } = null!;

    public string OwnershipDocumentFileUrl { get; set; } = null!;

    public string OwnerNationalApprovalFileUrl { get; set; } = null!;

    public string LocationName { get; set; } = null!;

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public string BuildingType { get; set; } = null!;

    public string BuildingNumber { get; set; } = null!;

    public string FloorNumber { get; set; } = null!;

    public string ApartmentNumber { get; set; } = null!;

    public string StreetNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsAccepted { get; set; }

    public bool IsRejected { get; set; }

    public DateTime SubmitDate { get; set; }

    public DateTime? ReviewDate { get; set; }

    public int ProviderId { get; set; }

    public bool IsActive { get; set; }

    public string? ReviewDescription { get; set; }
}
