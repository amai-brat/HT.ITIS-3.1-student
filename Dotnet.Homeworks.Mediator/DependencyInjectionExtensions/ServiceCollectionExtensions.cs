using System.Reflection;
using Dotnet.Homeworks.Mediator.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] handlersAssemblies)
    {
        var helper = new ReflectionHelper(handlersAssemblies);
        services.AddHandlers(helper.RequestHandlers, ServiceLifetime.Transient);
        services.AddTransient<IMediator, Mediator>();

        return services;
    }

    public static IServiceCollection AddPipelineBehaviorsForFeaturesNamespace(
        this IServiceCollection services, 
        string @namespace,  
        Assembly namespaceAssembly, 
        Assembly pipelineBehaviorsAssembly)
    {
        var pipes = PipelineBehaviorFinder.FindPipelineBehaviorsInNamespace(
            @namespace, 
            namespaceAssembly, 
            pipelineBehaviorsAssembly);
        
        foreach (var (iface, impl) in pipes)
        {
            services.Add(new ServiceDescriptor(iface, impl, ServiceLifetime.Transient));
        }
        
        return services;
    }
    
    private static IServiceCollection AddHandlers(this IServiceCollection services, 
        IEnumerable<(Type, Type)> handlers, 
        ServiceLifetime lifetime)
    {
        foreach (var (iface, impl) in handlers)
        {
            services.Add(new ServiceDescriptor(iface, impl, lifetime));
        }

        return services;
    }
}