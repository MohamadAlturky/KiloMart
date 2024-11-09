namespace KiloMart.Presentation.Models.Commands.Products;

public class CreateProductRequest
{
    public IFormFile? File { get; set; }
    public int CategoryId { get; set; }
    public CreateProductLocalizationRequest ArabicData { get; set; } = new();
    public CreateProductLocalizationRequest EnglishData { get; set; } = new();

    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();
        if (CategoryId == 0)
            errors.Add("Category is required");
        if (ArabicData is null)
            errors.Add("Arabic data is required");
        if (EnglishData is null)
            errors.Add("English data is required");
        if (ArabicData?.Validate().Success == false)
            errors.AddRange(ArabicData.Validate().Errors);
        if (EnglishData?.Validate().Success == false)
            errors.AddRange(EnglishData.Validate().Errors);
        if (File is null)
            errors.Add("File is required");
        return (errors.Count == 0, errors.ToArray());
    }
}
