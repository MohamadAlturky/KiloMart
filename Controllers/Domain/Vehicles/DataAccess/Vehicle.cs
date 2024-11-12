namespace KiloMart.Domain.Vehicles.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Year { get; set; } = null!;
    public int Delivary { get; set; }
}
