namespace KiloMart.Presentation.Models.Queries;

public class CardApiResponse
{
    public int Id { get; set; }
    public string HolderName { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string SecurityCode { get; set; } = string.Empty;
    public DateTime ExpireDate { get; set; }
    public int CustomerId { get; set; }
}