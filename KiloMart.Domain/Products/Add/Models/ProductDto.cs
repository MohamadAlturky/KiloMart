namespace KiloMart.Domain.Products.Add.Models;

public class ProductDto
{
    //write the public feild id , imagUrl , categoryId , isActive , MeasurementUnit , name and Description
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public bool IsActive { get; set; }
    public string MeasurementUnit { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    //write the public feild Localizations as list of ProductLocalizedDto
    public List<ProductLocalizedDto> Localizations { get; set; } = new();
    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(Name))
            errors.Add("Name is required");
        if (string.IsNullOrEmpty(Description))
            errors.Add("Description is required");
        if (CategoryId == 0)
            errors.Add("Category is required");
        if (string.IsNullOrEmpty(MeasurementUnit))
            errors.Add("Measurement unit is required");
        if (Localizations.Count == 0)
            errors.Add("At least one localization is required");
        if (Localizations.Any(l => string.IsNullOrEmpty(l.Name)))
            errors.Add("Localization name is required");
        if (Localizations.Any(l => string.IsNullOrEmpty(l.Description)))
            errors.Add("Localization description is required");
        if (Localizations.Any(l => l.Language == 0))
            errors.Add("Localization language is required");
        if (Localizations.Any(l => string.IsNullOrEmpty(l.MeasurementUnit)))
            errors.Add("Localization measurement unit is required");
        return (errors.Count == 0, errors.ToArray());
    }
}

