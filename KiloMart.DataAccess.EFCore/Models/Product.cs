using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Product
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int ProductCategory { get; set; }

    public bool IsActive { get; set; }

    public string MeasurementUnit { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ProductCategory ProductCategoryNavigation { get; set; } = null!;

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();

    public virtual ICollection<ProductLocalized> ProductLocalizeds { get; set; } = new List<ProductLocalized>();

    public virtual ICollection<ProductOffer> ProductOffers { get; set; } = new List<ProductOffer>();
}
