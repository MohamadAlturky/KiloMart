using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using KiloMart.DataAccess.EFCore.Models;

namespace KiloMart.DataAccess.EFCore.Configuration;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEFDataAccess(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<KiloMartMasterDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            // Apply global no-tracking behavior
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }
}