namespace KiloMart.Domain.Products.Offers.Models;
public class ProductOfferDto
{
    public int Id { get; set; }
    public int Product { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal Quantity { get; set; }
    public int Provider { get; set; }
    public bool IsActive { get; set; }
}
