using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductCategory
{
    public int Id { get; set; }

    public bool IsActive { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductCategoryLocalized> ProductCategoryLocalizeds { get; set; } = new List<ProductCategoryLocalized>();

    public virtual ICollection<ProductRequest> ProductRequests { get; set; } = new List<ProductRequest>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
