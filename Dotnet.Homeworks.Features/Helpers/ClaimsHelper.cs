using System.Security.Claims;

namespace Dotnet.Homeworks.Features.Helpers;

public static class ClaimsHelper
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        
        return Guid.TryParse(claim?.Value, out var result) 
            ? result 
            : null;
    }
}