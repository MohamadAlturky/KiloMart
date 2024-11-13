namespace KiloMart.Domain.DiscountCodes.Models;

public class CreateDiscountCodeRequest
{
    public string Code { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public byte DiscountType { get; set; }


    public (bool Success, string[] Errors) Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Code))
        {
            errors.Add("Discount code is required");
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            errors.Add("Description is required");
        }

        if (Value <= 0)
        {
            errors.Add("Value must be greater than 0");
        }
        
        if (EndDate.HasValue && StartDate > EndDate)
        {
            errors.Add("Start date cannot be after end date");
        }

        if (DiscountType == 0)
        {
            errors.Add("Invalid discount type");
        }

        return (errors.Count == 0, errors.ToArray());
    }
}