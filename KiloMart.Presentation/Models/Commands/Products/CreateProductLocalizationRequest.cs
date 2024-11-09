namespace KiloMart.Presentation.Models.Commands.Products;

public class CreateProductLocalizationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = string.Empty;
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(Name))
            errors.Add("Name is required");
        if (string.IsNullOrEmpty(Description))
            errors.Add("Description is required");
        if (string.IsNullOrEmpty(MeasurementUnit))
            errors.Add("Measurement unit is required");
        return (errors.Count == 0, errors.ToArray());
    }
}
