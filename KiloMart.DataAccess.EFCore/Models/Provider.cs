using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Provider
{
    public int Party { get; set; }

    public virtual ICollection<OrderProviderInformation> OrderProviderInformations { get; set; } = new List<OrderProviderInformation>();

    public virtual Party PartyNavigation { get; set; } = null!;

    public virtual ICollection<ProductOffer> ProductOffers { get; set; } = new List<ProductOffer>();

    public virtual ICollection<ProductRequest> ProductRequests { get; set; } = new List<ProductRequest>();

    public virtual ICollection<ProviderActivity> ProviderActivities { get; set; } = new List<ProviderActivity>();

    public virtual ICollection<ProviderDocument> ProviderDocuments { get; set; } = new List<ProviderDocument>();

    public virtual ProviderProfile? ProviderProfile { get; set; }

    public virtual ICollection<ProviderWallet> ProviderWallets { get; set; } = new List<ProviderWallet>();
}
