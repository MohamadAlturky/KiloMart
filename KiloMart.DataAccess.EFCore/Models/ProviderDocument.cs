using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class ProviderDocument
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public byte DocumentType { get; set; }

    public string Url { get; set; } = null!;

    public int Provider { get; set; }

    public virtual DocumentType DocumentTypeNavigation { get; set; } = null!;

    public virtual Provider ProviderNavigation { get; set; } = null!;
}
