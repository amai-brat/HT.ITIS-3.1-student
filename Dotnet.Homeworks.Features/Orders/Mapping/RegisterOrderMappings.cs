using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Orders.Queries.GetOrder;
using Dotnet.Homeworks.Features.Orders.Queries.GetOrders;
using Mapster;

namespace Dotnet.Homeworks.Features.Orders.Mapping;

public class RegisterOrderMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, GetOrderDto>();
        config.NewConfig<IEnumerable<Order>, GetOrdersDto>()
            .Map(dest => dest.Orders, src => src);
    }
}