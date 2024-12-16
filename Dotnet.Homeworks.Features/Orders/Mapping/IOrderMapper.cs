using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Orders.Queries.GetOrder;
using Dotnet.Homeworks.Features.Orders.Queries.GetOrders;
using Mapster;

namespace Dotnet.Homeworks.Features.Orders.Mapping;

[Mapper]
public interface IOrderMapper
{
    GetOrderDto MapToGetOrderDto(Order order);
    GetOrdersDto MapToGetOrdersDto(IEnumerable<Order> orders);
}