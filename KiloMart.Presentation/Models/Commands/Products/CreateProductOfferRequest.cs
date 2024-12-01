namespace KiloMart.Presentation.Models.Commands.Products;

public class CreateProductOfferRequest
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public decimal Quantity { get; set; }

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (ProductId <= 0)
            errors.Add("Product ID is required");
        if (Price <= 0)
            errors.Add("Price must be greater than zero");
        if (OffPercentage < 0 || OffPercentage > 100)
            errors.Add("Off percentage must be between 0 and 100");
        if (FromDate.AddMinutes(1) < DateTime.UtcNow)
            errors.Add("From date must be in the future");
        if (Quantity <= 0)
            errors.Add("Quantity must be greater than zero");

        return (errors.Count == 0, errors.ToArray());
    }
}

