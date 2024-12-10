using Dotnet.Homeworks.Features.Orders.PermissionChecks;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;

namespace Dotnet.Homeworks.Features.Orders.Queries.GetOrder;

public class GetOrderQuery : IQuery<GetOrderDto>, IOwnedOrderRequest
{
    public GetOrderQuery(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; init; }
}