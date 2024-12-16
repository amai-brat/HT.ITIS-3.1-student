using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Helpers;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Shared.Dto;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, CreateOrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly HttpContext _httpContext;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository, 
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<Result<CreateOrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _httpContext.User.GetUserId();
            if (userId is null)
            {
                return new Result<CreateOrderDto>(null, false, "User is not logged in");    
            }
            var user = await _userRepository.GetUserByGuidAsync(userId.Value, cancellationToken);
            if (user is null)
            {
                return new Result<CreateOrderDto>(null, false, "User not found");
            }
            
            var order = new Order
            {
                OrdererId = userId.Value,
                ProductsIds = request.ProductsIds
            };

            var id = await _orderRepository.InsertOrderAsync(order, cancellationToken);
            return new Result<CreateOrderDto>(new CreateOrderDto(id), true);
        }
        catch (Exception ex)
        {
            return new Result<CreateOrderDto>(null, false, ex.Message);
        }
    }
}