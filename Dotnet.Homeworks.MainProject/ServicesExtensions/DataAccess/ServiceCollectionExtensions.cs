using Dotnet.Homeworks.Data.DatabaseContext;
using Dotnet.Homeworks.DataAccess.Repositories;
using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));
        
        return services;
    }
}