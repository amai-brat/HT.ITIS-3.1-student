using Dotnet.Homeworks.Features.Orders.PermissionChecks;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;

namespace Dotnet.Homeworks.Features.Orders.Commands.DeleteOrder;

public class DeleteOrderByGuidCommand : ICommand, IOwnedOrderRequest
{
    public DeleteOrderByGuidCommand(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; init; }
}