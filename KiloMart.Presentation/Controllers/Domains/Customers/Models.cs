namespace KiloMart.Presentation.Controllers.Domains.Customers;
public class CartItemRequest
{
    public int Product { get; set; }
    public decimal Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    public int? Product { get; set; }
    public decimal? Quantity { get; set; }
}