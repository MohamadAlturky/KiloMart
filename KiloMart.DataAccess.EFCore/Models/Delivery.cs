using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Delivery
{
    public int Party { get; set; }

    public virtual ICollection<DelivaryDocument> DelivaryDocuments { get; set; } = new List<DelivaryDocument>();

    public virtual ICollection<DelivaryProfile> DelivaryProfiles { get; set; } = new List<DelivaryProfile>();

    public virtual Party PartyNavigation { get; set; } = null!;

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
