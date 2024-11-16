using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProviderProfile
{
    public int Id { get; set; }

    public int Provider { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string OwnerNationalId { get; set; } = null!;

    public string NationalApprovalId { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public virtual Provider ProviderNavigation { get; set; } = null!;
}
