namespace KiloMart.Api.Controllers.Queries;

public class ProductCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    // Localized properties
    public string? LocalizedName { get; set; }
    public byte? Language { get; set; }
}
