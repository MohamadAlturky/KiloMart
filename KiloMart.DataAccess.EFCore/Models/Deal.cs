using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Deal
{
    public int Id { get; set; }

    public int Product { get; set; }

    public bool IsActive { get; set; }

    public double OffPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual Product ProductNavigation { get; set; } = null!;
}
