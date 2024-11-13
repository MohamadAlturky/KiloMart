using KiloMart.Domain.Languages.Models;

namespace KiloMart.Domain.ProductCategories.Models;

public class CreateProductCategoryRequest
{
    public Language Language { get; set; }
    public string Name { get; set; } = string.Empty;
}