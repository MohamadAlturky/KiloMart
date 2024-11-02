namespace KiloMart.Domain.Models;

public class ProductDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public int ProductCategoryId { get; set; }
    public bool IsActive { get; set; }
}
