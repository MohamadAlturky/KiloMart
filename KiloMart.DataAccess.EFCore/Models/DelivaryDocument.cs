using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DelivaryDocument
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int Delivary { get; set; }

    public byte DocumentType { get; set; }

    public virtual Delivery DelivaryNavigation { get; set; } = null!;

    public virtual DocumentType DocumentTypeNavigation { get; set; } = null!;
}
