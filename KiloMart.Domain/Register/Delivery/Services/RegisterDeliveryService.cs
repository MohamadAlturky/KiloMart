using KiloMart.Domain.Register.Services;
using KiloMart.Domain.Register.Utils;

namespace KiloMart.Domain.Register.Delivery.Services;

public class RegisterDeliveryService : BaseRegisterService
{
    protected override string PartyTypeTableName => "Delivery";
    protected override UserRole UserRole => UserRole.Delivery;
}
