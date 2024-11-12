using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = (await _productRepository.GetAllProductsAsync(cancellationToken)).ToList();
            var dto = new GetProductsDto(products.Select(x => new GetProductDto(x.Id, x.Name)));
        
            return new Result<GetProductsDto>(dto, true);
        }
        catch (Exception e)
        {
            return new Result<GetProductsDto>(null, false, e.Message);
        }
    }
}