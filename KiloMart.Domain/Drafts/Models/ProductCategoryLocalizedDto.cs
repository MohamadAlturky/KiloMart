namespace KiloMart.Domain.Models;

public class ProductCategoryLocalizedDto
{
    public byte LanguageId { get; set; }
    public int ProductCategoryId { get; set; }
    public string Name { get; set; }
}
