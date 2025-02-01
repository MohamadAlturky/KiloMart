using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Delivery
{
    public int Party { get; set; }

    public virtual ICollection<DelivaryDocument> DelivaryDocuments { get; set; } = new List<DelivaryDocument>();

    public virtual DelivaryProfile? DelivaryProfile { get; set; }

    public virtual ICollection<DeliveryActivity> DeliveryActivities { get; set; } = new List<DeliveryActivity>();

    public virtual ICollection<DeliveryWallet> DeliveryWallets { get; set; } = new List<DeliveryWallet>();

    public virtual ICollection<OrderDeliveryInformation> OrderDeliveryInformations { get; set; } = new List<OrderDeliveryInformation>();

    public virtual Party PartyNavigation { get; set; } = null!;

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
