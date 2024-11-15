
using KiloMart.Domain.ProductRequests;
using Microsoft.AspNetCore.Http;

namespace KiloMart.Presentation.Models.Commands.ProductRequests;

public class ProductRequestInsertWithFileModel : ProductRequestInsertModel
{
    /// <summary>
    /// Image file for the product
    /// </summary>
    public IFormFile? ImageFile { get; set; }

    public new (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        // Call base class validation
        var (baseSuccess, baseErrors) = base.Validate();
        if (!baseSuccess)
        {
            errors.AddRange(baseErrors);
        }

        // Additional validation for ImageFile
        if (ImageFile == null && string.IsNullOrWhiteSpace(ImageUrl))
            errors.Add("Either ImageFile or ImageUrl must be provided.");

        return (errors.Count == 0, errors.ToArray());
    }
}
