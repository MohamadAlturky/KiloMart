using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class CustomerProfile
{
    public int Id { get; set; }

    public int Customer { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string NationalName { get; set; } = null!;

    public string NationalId { get; set; } = null!;

    public virtual Customer CustomerNavigation { get; set; } = null!;
}
