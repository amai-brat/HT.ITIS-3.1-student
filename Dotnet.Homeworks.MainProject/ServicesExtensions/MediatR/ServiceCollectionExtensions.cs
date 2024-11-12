using Dotnet.Homeworks.Features.Helpers;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.MediatR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(AssemblyReference.Assembly);
        });
        
        return services;
    }
}