using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProductRequestStatus
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductRequest> ProductRequests { get; set; } = new List<ProductRequest>();
}
