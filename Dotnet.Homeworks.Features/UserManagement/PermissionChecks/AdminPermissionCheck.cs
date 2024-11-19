using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.Enums;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.UserManagement.PermissionChecks;

public class AdminPermissionCheck : IPermissionCheck<IAdminRequest>
{
    private readonly HttpContext _httpContext;

    public AdminPermissionCheck(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public Task<IEnumerable<PermissionResult>> CheckPermissionAsync(IAdminRequest request)
    {
        var claims = _httpContext.User.Claims;
        if (claims.Any(x => x.Type == ClaimTypes.Role &&
                            x.Value == Roles.Admin.ToString()))
        {
            return Task.FromResult(new []{new PermissionResult(true)}.AsEnumerable());
        }
        
        return Task.FromResult(new []{new PermissionResult(false, "Admin role is required")}.AsEnumerable());
    }
}