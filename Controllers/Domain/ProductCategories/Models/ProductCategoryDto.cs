namespace KiloMart.Domain.ProductCategories.Models;

public class ProductCategoryDto
{
    public int Id { get; set; } // Primary Key for ProductCategory
    public bool IsActive { get; set; } // Indicates if the category is active
    public string Name { get; set; } = string.Empty; // Name of the category

    // List of localized names for this product category
    public List<ProductCategoryLocalizedDto> Localizations { get; set; } = new();

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required");

        if (Localizations.Count == 0)
            errors.Add("At least one localization is required");

        if (Localizations.Any(l => string.IsNullOrWhiteSpace(l.Name)))
            errors.Add("Localization name is required");
        if (Localizations.Any(l => l.Language == 0))
            errors.Add("Language is required");
        return (errors.Count == 0, errors.ToArray());
    }
}
