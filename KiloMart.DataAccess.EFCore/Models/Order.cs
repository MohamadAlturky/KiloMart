using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Order
{
    public long Id { get; set; }

    public byte OrderStatus { get; set; }

    public decimal TotalPrice { get; set; }

    public string TransactionId { get; set; } = null!;

    public int CustomerLocation { get; set; }

    public int ProviderLocation { get; set; }

    public int Customer { get; set; }

    public int Provider { get; set; }

    public virtual Location CustomerLocationNavigation { get; set; } = null!;

    public virtual Customer CustomerNavigation { get; set; } = null!;

    public virtual ICollection<OrderActivity> OrderActivities { get; set; } = new List<OrderActivity>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus OrderStatusNavigation { get; set; } = null!;

    public virtual Location ProviderLocationNavigation { get; set; } = null!;

    public virtual Provider ProviderNavigation { get; set; } = null!;

    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
}
