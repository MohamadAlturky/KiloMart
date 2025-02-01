using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DiscountCode
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public decimal Value { get; set; }

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public byte DiscountType { get; set; }

    public bool IsActive { get; set; }

    public virtual DiscountType DiscountTypeNavigation { get; set; } = null!;

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();

    public virtual ICollection<ProductOfferDiscount> ProductOfferDiscounts { get; set; } = new List<ProductOfferDiscount>();

    public virtual ICollection<OrderProductOffer> OrderItems { get; set; } = new List<OrderProductOffer>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
