using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Register.Services;
using KiloMart.Domain.Register.Utils;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Domain.Register.Delivery.Services;

public class RegisterDeliveryService : BaseRegisterService
{
    protected override string PartyTypeTableName => "Delivery";
    protected override UserRole UserRole => UserRole.Delivery;
}
