using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Location
{
    public int Id { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public string Name { get; set; } = null!;

    public int Party { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<LocationDetail> LocationDetails { get; set; } = new List<LocationDetail>();

    public virtual ICollection<Order> OrderCustomerLocationNavigations { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderProviderLocationNavigations { get; set; } = new List<Order>();

    public virtual Party PartyNavigation { get; set; } = null!;
}
