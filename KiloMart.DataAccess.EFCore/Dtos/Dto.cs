namespace KiloMart.DataAccess.EFCore.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string MeasurementUnit { get; set; } = null!;
}


public class ProviderDto
{
    public int PartyId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;

}
public class ProductOfferDto
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public decimal OffPercentage { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public double Quantity { get; set; }
    public ProductDto Product { get; set; } = null!;
    public ProviderDto Provider { get; set; } = null!;
}
public class OrderProductOfferDto
{
    public ProductOfferDto ProductOffer { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public double Quantity { get; set; }
    public List<DiscountCodeDto> DiscountCodes { get; set; } = new();
}
public class OrderProductDto
{
    public ProductDto Product { get; set; } = null!;
    public double Quantity { get; set; }
}
public class CustomerDto
{
    public int PartyId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
public class LocationDto
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    public required double Lat { get; set; }
    public required double Lng { get; set; }
}
public class OrderCustomerInformationDto
{
    public CustomerDto Customer { get; set; } = null!;
    public LocationDto Location { get; set; } = null!;
}
public class DeliveryDto
{
    public int PartyId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
public class OrderDeliveryInformationDto
{
    public DeliveryDto Delivery { get; set; } = null!;
}
public class OrderProviderInformationDto
{
    public ProviderDto Provider { get; set; } = null!;
    public LocationDto Location { get; set; } = null!;
}
public class OrderActivityDto
{
    public DateTime Date { get; set; }
    public string OrderActivityType { get; set; } = null!;
    public string OperatedBy { get; set; } = null!;
}
public class DiscountCodeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public decimal Value { get; set; }
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string DiscountType { get; set; } = null!;
}
public class OrderDto
{
    public long Id { get; set; }
    public string OrderStatus { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public string TransactionId { get; set; } = null!;
    public DateTime Date { get; set; }
    public string PaymentType { get; set; } = null!;
    public bool IsPaid { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal SystemFee { get; set; }
    public decimal ItemsPrice { get; set; }
    public string? SpecialRequest { get; set; }
    public List<OrderProductDto> OrderProducts { get; set; } = new();
    public List<OrderProductOfferDto> OrderProductOffers { get; set; } = new();
    public List<OrderCustomerInformationDto> OrderCustomerInformations { get; set; } = new();
    public List<OrderDeliveryInformationDto> OrderDeliveryInformations { get; set; } = new();
    public List<OrderProviderInformationDto> OrderProviderInformations { get; set; } = new();
    public List<OrderActivityDto> OrderActivities { get; set; } = new();
    public List<DiscountCodeDto> DiscountCodes { get; set; } = new();
}