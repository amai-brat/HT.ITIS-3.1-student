using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderCommandValidator(IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;

        RuleFor(x => x.OrderId)
            .MustAsync(IsOrderExistAsync)
            .WithMessage("Order not found");
        
        RuleForEach(x => x.ProductsIds)
            .MustAsync(IsProductExistAsync)
            .WithMessage("Product not found");
        
        RuleFor(x => x.ProductsIds.Count())
            .GreaterThan(0)
            .WithMessage("Products count must be greater than 0");
    }

    private async Task<bool> IsOrderExistAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByGuidAsync(orderId, cancellationToken);
        return order != null;
    }

    private async Task<bool> IsProductExistAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(productId, cancellationToken);
        return product != null;
    }
}