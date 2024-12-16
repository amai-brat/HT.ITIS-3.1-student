using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

public interface IPermissionChecker
{
    Task<TResponse> Check<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);
}

public interface IPermissionCheck<in TRequest>
{
    Task<IEnumerable<PermissionResult>> CheckPermissionAsync(TRequest request);
}