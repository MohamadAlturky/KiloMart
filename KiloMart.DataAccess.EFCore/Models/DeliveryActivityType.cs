using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DeliveryActivityType
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DeliveryActivity> DeliveryActivities { get; set; } = new List<DeliveryActivity>();

    public virtual ICollection<ProviderActivity> ProviderActivities { get; set; } = new List<ProviderActivity>();
}
