using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class PhoneNumber
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public int Party { get; set; }

    public bool IsActive { get; set; }

    public virtual Party PartyNavigation { get; set; } = null!;
}
