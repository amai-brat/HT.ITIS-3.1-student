using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Products.Mapping;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Commands.InsertProduct;

internal sealed class InsertProductCommandHandler : ICommandHandler<InsertProductCommand, InsertProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductMapper _productMapper;

    public InsertProductCommandHandler(
        IProductRepository productRepository, 
        IUnitOfWork unitOfWork,
        IProductMapper productMapper)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _productMapper = productMapper;
    }

    public async Task<Result<InsertProductDto>> Handle(InsertProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = _productMapper.MapToProduct(request);
        
            var id = await _productRepository.InsertProductAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Result<InsertProductDto>(new InsertProductDto(id), true);
        }
        catch (Exception e)
        {
            return new Result<InsertProductDto>(null, false, e.Message);
        }
    }
}