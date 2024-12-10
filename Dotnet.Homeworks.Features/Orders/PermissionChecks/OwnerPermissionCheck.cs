using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Helpers;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Orders.PermissionChecks;

public class OwnerPermissionCheck : IPermissionCheck<IOwnedOrderRequest>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly HttpContext _httpContext;
    
    public OwnerPermissionCheck(
        IHttpContextAccessor httpContextAccessor,
        IOrderRepository orderRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    public async Task<IEnumerable<PermissionResult>> CheckPermissionAsync(IOwnedOrderRequest request)
    {
        var userId = _httpContext.User.GetUserId();
        if (userId is null)
        {
            return new [] { new PermissionResult(false, "User is not logged in") };
        }

        var user = await _userRepository.GetUserByGuidAsync(userId.Value, default);
        if (user is null)
        {
            return new [] { new PermissionResult(false, "User not found") };
        }
        
        var order = await _orderRepository.GetOrderByGuidAsync(request.OrderId, default);
        if (order is null)
        {
            return new [] { new PermissionResult(false, "Order not found") };
        }
        
        return order.OrdererId == userId 
            ? new[] { new PermissionResult(true) } 
            : new[] { new PermissionResult(false, "User does not own this order.") };
    }
}