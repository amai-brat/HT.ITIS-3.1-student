using System.Reflection;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        params Assembly[] assemblies
    )
    {
        var tuples = PermissionChecker.GetPermissionChecksFrom(assemblies);
        tuples.ForEach(tuple =>
        {
            serviceCollection.Add(new ServiceDescriptor(tuple.Iface, tuple.Impl, ServiceLifetime.Scoped));
        });

        serviceCollection.AddScoped<IPermissionChecker, PermissionChecker>();
    }
}