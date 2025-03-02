namespace KiloMart.Domain.Customers.Profile;


public class CreateCustomerProfileRequest
{
    public int Customer { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
}

public class CreateCustomerProfileResponse
{
    public int Id { get; set; }
    public int Customer { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
}


public class UpdateCustomerProfileResponse
{
    public int CustomerId { get; set; } // Identifier of the updated customer profile
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string NationalName { get; set; }
    public string NationalId { get; set; }
}

public class UpdateCustomerProfileRequest
{
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? NationalName { get; set; }
    public string? NationalId { get; set; }
}