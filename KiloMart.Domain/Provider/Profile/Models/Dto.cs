namespace KiloMart.Domain.Providers.Profile;

public class CreateProviderProfileRequest
{
    public int Provider { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string OwnerNationalId { get; set; }
    public string NationalApprovalId { get; set; }
    public string CompanyName { get; set; }
    public string OwnerName { get; set; }
}

public class CreateProviderProfileResponse
{
    public int Id { get; set; }
    public int Provider { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string OwnerNationalId { get; set; }
    public string NationalApprovalId { get; set; }
    public string CompanyName { get; set; }
    public string OwnerName { get; set; }
}

public class UpdateProviderProfileRequest
{
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? OwnerNationalId { get; set; }
    public string? NationalApprovalId { get; set; }
    public string? CompanyName { get; set; }
    public string? OwnerName { get; set; }
}

public class UpdateProviderProfileResponse
{
    public int ProviderId { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string OwnerNationalId { get; set; }
    public string NationalApprovalId { get; set; }
    public string CompanyName { get; set; }
    public string OwnerName { get; set; }
}
