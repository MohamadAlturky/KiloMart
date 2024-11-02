namespace KiloMart.Domain.Models;

public class ProductOfferDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal Quantity { get; set; }
    public int ProviderId { get; set; }
}
