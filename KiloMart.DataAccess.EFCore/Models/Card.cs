using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Card
{
    public int Id { get; set; }

    public string HolderName { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string SecurityCode { get; set; } = null!;

    public DateOnly ExpireDate { get; set; }

    public int Customer { get; set; }

    public bool IsActive { get; set; }

    public virtual Customer CustomerNavigation { get; set; } = null!;
}
