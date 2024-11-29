using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors;

public class PermissionCheckPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
{
    private readonly IPermissionChecker _permissionChecker;

    public PermissionCheckPipelineBehavior(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var res = await _permissionChecker.Check<TRequest, TResponse>(request, cancellationToken);
        if (res is Result { IsSuccess: true })
        {
            return await next();
        }

        return res;
    }
}