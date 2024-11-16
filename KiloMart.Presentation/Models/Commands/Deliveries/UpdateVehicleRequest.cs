namespace KiloMart.Presentation.Models.Commands.Deliveries;

public class UpdateVehicleRequest
{
    public string? Number { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
    public string? Year { get; set; }
}