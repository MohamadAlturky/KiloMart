namespace KiloMart.Domain.DiscountCodes.Models;

public class CreateDiscountCodeResponse
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public byte DiscountType { get; set; }
    public bool IsActive { get; set; }
}
