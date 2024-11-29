using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public class PermissionCheckDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IPermissionChecker _permissionChecker;

    protected PermissionCheckDecorator(IPermissionChecker permissionChecker)
    {
        _permissionChecker = permissionChecker;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var res = await _permissionChecker.Check<TRequest, TResponse>(request, cancellationToken);
        return res;
    }
}