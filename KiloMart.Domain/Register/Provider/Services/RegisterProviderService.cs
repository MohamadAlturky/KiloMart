using KiloMart.Domain.Register.Services;
using KiloMart.Domain.Register.Utils;

namespace KiloMart.Domain.Register.Provider.Services;

public class RegisterProviderService : BaseRegisterService
{
    protected override string PartyTypeTableName => "Provider";
    protected override UserRole UserRole => UserRole.Provider;
}
