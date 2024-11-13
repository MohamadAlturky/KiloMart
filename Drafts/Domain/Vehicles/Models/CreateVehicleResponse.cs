namespace KiloMart.Domain.Vehicles.Models;

public class CreateVehicleResponse
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public int Delivery { get; set; }
}
