namespace KiloMart.Domain.ProductCategories.Models;

public class ProductCategoryLocalizedDto
{
    public string Name { get; set; } = string.Empty; // Localized name of the category
    public byte Language { get; set; } // Language code
    public int ProductCategory { get; set; } // Foreign key reference to ProductCategory
}
