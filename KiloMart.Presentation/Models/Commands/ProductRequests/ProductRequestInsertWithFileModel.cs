namespace KiloMart.Presentation.Models.Commands.ProductRequests;

public class ProductRequestInsertWithFileModel
{
    /// <summary>
    /// Image file for the product
    /// </summary>                                                                                                                                                                
    public IFormFile? ImageFile { get; set; }
    public int ProductCategory { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public float Quantity { get; set; }

    /// <summary>
    /// Data
    /// </summary>
    public byte Language { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string MeasurementUnit { get; set; } = null!;


    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (ProductCategory <= 0)
            errors.Add("Product category must be a positive integer.");

        if (Price <= 0)
            errors.Add("Price must be greater than zero.");

        if (OffPercentage < 0 || OffPercentage > 100)
            errors.Add("Off percentage must be between 0 and 100.");

        if (Quantity <= 0)
            errors.Add("Quantity must be greater than zero.");

        if (Language < 1)
            errors.Add("Language must be specified.");

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(Description))
            errors.Add("Description is required.");

        if (string.IsNullOrWhiteSpace(MeasurementUnit))
            errors.Add("Measurement unit is required.");


        return (errors.Count == 0, errors.ToArray());
    }
}
