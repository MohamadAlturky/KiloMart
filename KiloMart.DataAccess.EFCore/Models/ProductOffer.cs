using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductOffer
{
    public int Id { get; set; }

    public int Product { get; set; }

    public decimal Price { get; set; }

    public decimal OffPercentage { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public double Quantity { get; set; }

    public int Provider { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<OrderProductOffer> OrderProductOffers { get; set; } = new List<OrderProductOffer>();

    public virtual Product ProductNavigation { get; set; } = null!;

    public virtual ICollection<ProductOfferDiscount> ProductOfferDiscounts { get; set; } = new List<ProductOfferDiscount>();

    public virtual Provider ProviderNavigation { get; set; } = null!;
}
