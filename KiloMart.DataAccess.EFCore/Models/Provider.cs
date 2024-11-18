using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Provider
{
    public int Party { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Party PartyNavigation { get; set; } = null!;

    public virtual ICollection<ProductOffer> ProductOffers { get; set; } = new List<ProductOffer>();

    public virtual ICollection<ProductRequest> ProductRequests { get; set; } = new List<ProductRequest>();

    public virtual ICollection<ProviderDocument> ProviderDocuments { get; set; } = new List<ProviderDocument>();

    public virtual ICollection<ProviderProfile> ProviderProfiles { get; set; } = new List<ProviderProfile>();
}
