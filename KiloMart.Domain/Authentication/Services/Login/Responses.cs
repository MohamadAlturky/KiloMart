namespace KiloMart.Presentation.Authentication.Services.Login;

public class CustomerData
{
    public string? Token { get; set; }
    public string? Role { get; set; }
    public Object? ActiveProfile { get; set; }
    public Object? UserInfo { get; set; }
    public Object? CustomerInfo { get; set; }
}

public class DeliveryData
{
    public string? Token { get; set; }
    public string? Role { get; set; }
    public Object? ActiveProfile { get; set; }
    public Object? UserInfo { get; set; }
    public Object? DeliveryInfo { get; set; }
    public Object? ProfilesHistory { get; set; }
}

public class ProviderData
{
    public string? Token { get; set; }
    public string? Role { get; set; }
    public Object? UserInfo { get; set; }
    public Object? ProviderInfo { get; set; }
    public Object? ActiveProfile { get; set; }
    public Object? ProfilesHistory { get; set; }
}

public class AdminData
{
    public string? Token { get; set; }
    public string? Role { get; set; }
    public Object? UserInfo { get; set; }
    public Object? AdminInfo { get; set; }
}
