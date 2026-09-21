using AKERP.Application.Abstractions;
using AKERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AKERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, bool useSqlite = true)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            if (useSqlite)
                options.UseSqlite(connectionString);
            else
                options.UseSqlServer(connectionString);
        });

        services.AddScoped<IFeatureService, FeatureService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILicenseService, LicenseService>();
        services.AddScoped<ICompanyConfigService, CompanyConfigService>();

        return services;
    }
}
