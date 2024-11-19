using System.Reflection;

namespace Dotnet.Homeworks.Mediator.Helpers;

public class ReflectionHelper
{
    public IReadOnlyDictionary<Type, Type> Binds { get; }
    public IReadOnlyList<(Type Iface, Type Impl)> Requests { get; }
    public IReadOnlyList<(Type Iface, Type Impl)> RequestHandlers { get; }

    public ReflectionHelper(IEnumerable<Assembly> assemblies)
    {
        var types = assemblies
            .SelectMany(x => x.GetTypes())
            .ToList();
        
        RequestHandlers = GetRequestHandlers(types).AsReadOnly();
        Requests = GetRequests(types).AsReadOnly();
        Binds = GetBinds().AsReadOnly();
    }

    private Dictionary<Type, Type> GetBinds()
    {
        var binds = new Dictionary<Type, Type>();
        foreach (var request in Requests)
        {
            binds[request.Iface] = GetHandlerType(request);
        }

        return binds;
    }

    private Type GetHandlerType((Type Iface, Type Impl) ifaceAndReq)
    {
        return ifaceAndReq.Iface.IsGenericType 
            ? GetHandlerTypeIfGenericRequest(ifaceAndReq) 
            : GetHandlerTypeIfNonGenericRequest(ifaceAndReq);
    }

    private Type GetHandlerTypeIfNonGenericRequest((Type Iface, Type Impl) ifaceAndReq)
    {
        return RequestHandlers
            .Single(x => 
                x.Iface.GenericTypeArguments.Length == 1 &&
                x.Iface.GenericTypeArguments[0] == ifaceAndReq.Impl)
            .Impl;
    }

    private Type GetHandlerTypeIfGenericRequest((Type Iface, Type Impl) ifaceAndReq)
    {
        return RequestHandlers
            .Single(x =>
                x.Iface.GenericTypeArguments.Length == 2 &&
                x.Iface.GenericTypeArguments[0] == ifaceAndReq.Impl)
            .Impl;
    }

    public static List<(Type Iface, Type Impl)> GetRequests(IEnumerable<Type> types)
    {
        Func<Type, bool> isRequestInterface = i =>
            i == typeof(IRequest) ||
            (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
        
        var requests = types
            .Where(x => x
                .GetInterfaces()
                .Any(isRequestInterface));

        return requests
            .Select(x => (
                IFace: x
                    .GetInterfaces()
                    .First(isRequestInterface), 
                Impl: x))
            .ToList();
    }

    public static List<(Type Iface, Type Impl)> GetRequestHandlers(IEnumerable<Type> types)
    {
        Func<Type, bool> isHandlerInterface = i =>
            i.IsGenericType &&
            (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
             i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
        
        var requestHandlers = types
            .Where(x => x
                .GetInterfaces()
                .Any(isHandlerInterface))
            .ToList();

        return requestHandlers
            .Select(x => (
                Iface: x
                    .GetInterfaces()
                    .First(isHandlerInterface),
                Impl: x))
            .ToList();
    }
}