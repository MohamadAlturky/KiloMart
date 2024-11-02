using KiloMart.DataAccess.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace KiloMart.DataAccess.Configurations;

public static class DependencyInjection
{
    public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddSingleton<IDbFactory>(_ =>
        {
            return new DbFactory(configuration.GetConnectionString("DefaultConnection")!);
        });
    }
}
