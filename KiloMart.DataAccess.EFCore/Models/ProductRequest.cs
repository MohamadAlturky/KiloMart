using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductRequest
{
    public int Id { get; set; }

    public int Provider { get; set; }

    public DateTime Date { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int ProductCategory { get; set; }

    public decimal Price { get; set; }

    public decimal OffPercentage { get; set; }

    public decimal Quantity { get; set; }

    public byte Status { get; set; }

    public virtual ProductCategory ProductCategoryNavigation { get; set; } = null!;

    public virtual ICollection<ProductRequestDataLocalized> ProductRequestDataLocalizeds { get; set; } = new List<ProductRequestDataLocalized>();

    public virtual Provider ProviderNavigation { get; set; } = null!;

    public virtual ProductRequestStatus StatusNavigation { get; set; } = null!;
}
