namespace KiloMart.Domain.Models;

public class ProductLocalizedDto
{
    public byte LanguageId { get; set; }
    public int ProductId { get; set; }
    public string MeasurementUnit { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
}
