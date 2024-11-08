namespace KiloMart.Domain.Vehicles.Models;

public class CreateVehicleRequest
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public int Delivery { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(Number))
            errors.Add("Vehicle number is required");
        if (string.IsNullOrEmpty(Model))
            errors.Add("Vehicle model is required");
        if (string.IsNullOrEmpty(Type))
            errors.Add("Vehicle type is required");
        if (string.IsNullOrEmpty(Year))
            errors.Add("Vehicle year is required");
        if (Delivery <= 0)
            errors.Add("Valid delivery ID is required");

        return (errors.Count == 0, errors.ToArray());
    }
}
