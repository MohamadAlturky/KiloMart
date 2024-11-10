namespace KiloMart.Domain.Customers.Profile.Models;


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


