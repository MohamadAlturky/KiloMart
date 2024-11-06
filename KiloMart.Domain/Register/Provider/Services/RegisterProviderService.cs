using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Register.Services;
using KiloMart.Domain.Register.Utils;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Domain.Register.Provider.Services;

public class RegisterProviderService : BaseRegisterService
{
    protected override string PartyTypeTableName => "Provider";
    protected override UserRole UserRole => UserRole.Provider;
}
