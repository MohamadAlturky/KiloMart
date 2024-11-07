using KiloMart.Domain.Languages.Models;

namespace KiloMart.Api.Controllers;

public class CreateProductCategoryRequest
{
    public Language Language { get; set; }
    public string Name { get; set; } = string.Empty;
}