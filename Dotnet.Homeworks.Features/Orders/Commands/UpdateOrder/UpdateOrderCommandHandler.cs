using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Helpers;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Shared.Dto;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly HttpContext _httpContext;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor)
    {
        _orderRepository = orderRepository;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<Result> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _httpContext.User.GetUserId();
            var order = new Order
            {
                Id = request.OrderId,
                OrdererId = userId!.Value,
                ProductsIds = request.ProductsIds
            };
            
            await _orderRepository.UpdateOrderAsync(order, cancellationToken);
            return new Result(true);
        }
        catch (Exception ex)
        {
            return new Result(false, ex.Message);
        }
    }
}