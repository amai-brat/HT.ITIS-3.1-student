using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Features.Products.Mapping;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductMapper _productMapper;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IProductMapper productMapper)
    {
        _productRepository = productRepository;
        _productMapper = productMapper;
    }

    public async Task<Result<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllProductsAsync(cancellationToken);
            var dto = _productMapper.MapToGetProductsDto(products);
        
            return new Result<GetProductsDto>(dto, true);
        }
        catch (Exception e)
        {
            return new Result<GetProductsDto>(null, false, e.Message);
        }
    }
}