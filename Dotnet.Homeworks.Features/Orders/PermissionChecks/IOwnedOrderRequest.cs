using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;

namespace Dotnet.Homeworks.Features.Orders.PermissionChecks;

public interface IOwnedOrderRequest : ISecuredRequest
{
    public Guid OrderId { get; }
}