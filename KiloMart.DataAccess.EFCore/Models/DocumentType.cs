using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class DocumentType
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DelivaryDocument> DelivaryDocuments { get; set; } = new List<DelivaryDocument>();

    public virtual ICollection<ProviderDocument> ProviderDocuments { get; set; } = new List<ProviderDocument>();
}
