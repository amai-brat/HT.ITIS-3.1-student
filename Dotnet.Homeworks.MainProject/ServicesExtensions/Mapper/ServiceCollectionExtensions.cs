using System.Reflection;
using Mapster;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Mapper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services, Assembly mapperConfigsAssembly)
    {
        var types = mapperConfigsAssembly.GetTypes();
        var mapperIfaces = types
            .Where(x => x.GetCustomAttribute<MapperAttribute>() is not null);

        foreach (var mapperIface in mapperIfaces)
        {
            var mapper = types
                .Single(t => t.GetInterfaces()
                    .Any(i => i == mapperIface));
            services.Add(ServiceDescriptor.Singleton(mapperIface, mapper));
        }
        
        return services;
    }
}