using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Orders.Mapping;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Orders.Queries.GetOrder;

public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, GetOrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderMapper _orderMapper;

    public GetOrderQueryHandler(
        IOrderRepository orderRepository,
        IOrderMapper orderMapper)
    {
        _orderRepository = orderRepository;
        _orderMapper = orderMapper;
    }

    public async Task<Result<GetOrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetOrderByGuidAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                return new Result<GetOrderDto>(null, false, "Order not found");
            }
            
            var dto = _orderMapper.MapToGetOrderDto(order);
            return new Result<GetOrderDto>(dto, true);

        }
        catch (Exception ex)
        {
            return new Result<GetOrderDto>(null, false, ex.Message);
        }
    }
}