using KiloMart.Domain.Register.Services;
using KiloMart.Domain.Register.Utils;

namespace KiloMart.Domain.Register.Customer.Services;

public class RegisterCustomerService : BaseRegisterService
{
    protected override string PartyTypeTableName => "Customer";
    protected override Roles UserRole => Roles.Customer;
}
