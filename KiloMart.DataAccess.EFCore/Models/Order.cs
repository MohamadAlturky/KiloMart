using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Order
{
    public long Id { get; set; }

    public byte OrderStatus { get; set; }

    public decimal TotalPrice { get; set; }

    public string TransactionId { get; set; } = null!;

    public DateTime Date { get; set; }

    public byte PaymentType { get; set; }

    public bool IsPaid { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal SystemFee { get; set; }

    public decimal ItemsPrice { get; set; }

    public string? SpecialRequest { get; set; }

    public virtual ICollection<OrderActivity> OrderActivities { get; set; } = new List<OrderActivity>();

    public virtual ICollection<OrderCustomerInformation> OrderCustomerInformations { get; set; } = new List<OrderCustomerInformation>();

    public virtual ICollection<OrderDeliveryInformation> OrderDeliveryInformations { get; set; } = new List<OrderDeliveryInformation>();

    public virtual ICollection<OrderProductOffer> OrderProductOffers { get; set; } = new List<OrderProductOffer>();

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<OrderProviderInformation> OrderProviderInformations { get; set; } = new List<OrderProviderInformation>();

    public virtual OrderStatus OrderStatusNavigation { get; set; } = null!;

    public virtual PaymentType PaymentTypeNavigation { get; set; } = null!;

    public virtual ICollection<SystemActivity> SystemActivities { get; set; } = new List<SystemActivity>();

    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
}
