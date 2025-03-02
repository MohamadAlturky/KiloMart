namespace KiloMart.Domain.Products.Add.Models;

public class ProductLocalizedDto
{
    public int Product { get; set; }
    
    public byte Language { get; set; }
    public string MeasurementUnit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}