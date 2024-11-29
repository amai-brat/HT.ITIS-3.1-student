using System.Reflection;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

public class PermissionChecker : IPermissionChecker
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionChecker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<TResponse> Check<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
    {
        if (!typeof(TRequest)
                .GetInterfaces()
                .Any(x =>
                    x == typeof(IClientRequest) ||
                    x == typeof(IAdminRequest)))
        {
            return (TResponse)ResultFactory.Create(true, typeof(TResponse));
        }

        var permissionCheckIface = GetPermissionCheckIfaceType(typeof(TRequest));
        var permCheck = _serviceProvider.GetService(permissionCheckIface);
        var method = permissionCheckIface.GetMethod("CheckPermissionAsync", BindingFlags.Public | BindingFlags.Instance)!;
        var task = (Task<IEnumerable<PermissionResult>>)method.Invoke(permCheck, new object[] { request! })!;
        var permResults = await task;

        if (permResults.All(x => x.IsSuccess))
        {
            return (TResponse)ResultFactory.Create(true, typeof(TResponse));
        }
        
        return (TResponse)ResultFactory.Create(false, typeof(TResponse), null, "Not enough permisssions");
    }

    private Type GetPermissionCheckIfaceType(Type requestType)
    {
        var ifaces = new[]
        {
            requestType.GetInterface(nameof(IClientRequest)),
            requestType.GetInterface(nameof(IAdminRequest))
        };

        return typeof(IPermissionCheck<>)
            .MakeGenericType(ifaces.First(x => x is not null) 
                             ?? throw new InvalidOperationException("Request doesn't implement any interface to check permission"));
    }

    public static List<(Type Iface, Type Impl)> GetPermissionChecksFrom(params Assembly[] assemblies)
    {
        Func<Type, bool> isPermissionCheckInterface = t =>
            t.IsGenericType &&
            t.GetGenericTypeDefinition() == typeof(IPermissionCheck<>);
        
        var checks = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(x => x
                .GetInterfaces()
                .Any(isPermissionCheckInterface));
        
        return checks
            .Select(x => (
                Iface: x
                    .GetInterfaces()
                    .First(isPermissionCheckInterface), 
                Impl: x))
            .ToList();
    }
}
