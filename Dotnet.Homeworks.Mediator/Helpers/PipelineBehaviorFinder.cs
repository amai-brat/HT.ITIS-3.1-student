using System.Reflection;

namespace Dotnet.Homeworks.Mediator.Helpers;

public static class PipelineBehaviorFinder
{
    // e.g. (IPipelineBehavior<RequestA, ResponseA>, ValidationBehavior<RequestA, ResponseA>)
    public static List<(Type Iface, Type Impl)> FindPipelineBehaviorsInNamespace(
        string @namespace, 
        Assembly namespaceAssembly, 
        Assembly pipelineBehaviorsAssembly)
    {
        var types = namespaceAssembly.GetTypes();
        var typesInNamespace = types
            .Where(type => type.FullName != null && 
                           type.FullName.StartsWith(@namespace))
            .ToList();
        
        var requestHandlerTypes = ReflectionHelper.GetRequestHandlers(typesInNamespace);
        var reqAndResps = GetRequestAndResponseTuples(requestHandlerTypes.Select(x => x.Iface));
        var openPipeImpls = GetOpenPipelineBehaviorImpls(pipelineBehaviorsAssembly.GetTypes());
        
        var result = new List<(Type Iface, Type Impl)>();
        foreach (var (requestType, responseType) in reqAndResps)
        {
            var pipelineIface = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
            var pipelineImpls = openPipeImpls.Select(type => type.MakeGenericType(requestType, responseType));

            result.AddRange(pipelineImpls.Select(pipelineImpl => (pipelineIface, pipelineImpl)));
        }

        return result;
    }

    private static List<Type> GetOpenPipelineBehaviorImpls(IEnumerable<Type> types)
    {
        return types
            .Where(type => type
                .GetInterfaces()
                .Any(i => i.IsGenericType && 
                          i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>)))
            .ToList();
    }

    private static List<(Type RequestType, Type ResponseType)> GetRequestAndResponseTuples(
        IEnumerable<Type> closedRequestHandlerInterfaceTypes)
    {
        return closedRequestHandlerInterfaceTypes
            .Select(x => (RequestType: x.GetGenericArguments()[0], ResponseType: x.GetGenericArguments()[1]))
            .ToList();
    }
}