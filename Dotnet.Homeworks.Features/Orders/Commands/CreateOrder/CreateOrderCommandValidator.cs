using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        
        RuleForEach(x => x.ProductsIds)
            .MustAsync(IsProductExistAsync)
            .WithMessage("Product not found");
        
        RuleFor(x => x.ProductsIds)
            .Must(x => x.Any())
            .WithMessage("Order must contain at least one product");
    }

    private async Task<bool> IsProductExistAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(productId, cancellationToken);
        return product != null;
    }
}