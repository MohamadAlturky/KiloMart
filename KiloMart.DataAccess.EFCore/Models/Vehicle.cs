using System;
using System.Collections.Generic;

namespace KiloMart.DataAccess.EFCore.Models;

public partial class Vehicle
{
    public int Id { get; set; }

    public string Number { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Year { get; set; } = null!;

    public int Delivary { get; set; }

    public virtual Delivery DelivaryNavigation { get; set; } = null!;
}
