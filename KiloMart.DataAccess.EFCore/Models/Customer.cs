using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Customer
{
    public int Party { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    public virtual ICollection<CustomerProfile> CustomerProfiles { get; set; } = new List<CustomerProfile>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Party PartyNavigation { get; set; } = null!;
}
