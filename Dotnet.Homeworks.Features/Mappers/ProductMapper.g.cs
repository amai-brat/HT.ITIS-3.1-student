using System.Collections.Generic;
using System.Linq;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Products.Commands.InsertProduct;
using Dotnet.Homeworks.Features.Products.Commands.UpdateProduct;
using Dotnet.Homeworks.Features.Products.Mapping;
using Dotnet.Homeworks.Features.Products.Queries.GetProducts;

namespace Dotnet.Homeworks.Features.Products.Mapping
{
    public partial class ProductMapper : IProductMapper
    {
        public Product MapToProduct(InsertProductCommand p1)
        {
            return p1 == null ? null : new Product() {Name = p1.Name};
        }
        public Product MapToProduct(UpdateProductCommand p2)
        {
            return p2 == null ? null : new Product()
            {
                Name = p2.Name,
                Id = p2.Guid
            };
        }
        public GetProductsDto MapToGetProductsDto(IEnumerable<Product> p3)
        {
            return p3 == null ? null : new GetProductsDto(p3 == null ? null : p3.Select<Product, GetProductDto>(funcMain1));
        }
        
        private GetProductDto funcMain1(Product p4)
        {
            return p4 == null ? null : new GetProductDto(p4.Id, p4.Name);
        }
    }
}