using Dapper;
using KiloMart.DataAccess.Contracts;
using KiloMart.Domain.Languages.Models;
using KiloMart.Domain.Register.Services;
using KiloMart.Domain.Register.Utils;
using Microsoft.Extensions.Configuration;

namespace KiloMart.Domain.Register.Customer.Services;

public class RegisterCustomerService : BaseRegisterService
{
    protected override string PartyTypeTableName => "Customer";
    protected override UserRole UserRole => UserRole.Customer;
}
