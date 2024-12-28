using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderByGuidCommand>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderCommandValidator(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        
        RuleFor(x => x.OrderId)
            .MustAsync(IsOrderExistAsync)
            .WithMessage("Order not found");
    }

    private async Task<bool> IsOrderExistAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByGuidAsync(orderId, cancellationToken);
        return order is not null;
    }
}