using Ardalis.Specification;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.Parameters;

namespace Catalog.DAL.Specifications
{
    public class ProductWithFiltersSpecification : Specification<Product>
    {
        public ProductWithFiltersSpecification(ProductParameters parameters)
        {
            //filters
            if (parameters.CategoryId.HasValue)
                Query.Where(p => p.CategoryId == parameters.CategoryId.Value);

            if (parameters.MetalId.HasValue)
                Query.Where(p => p.MetalId == parameters.MetalId.Value);

            if (parameters.MinPrice.HasValue)
                Query.Where(p => p.Price >= parameters.MinPrice.Value);

            if (parameters.MaxPrice.HasValue)
                Query.Where(p => p.Price <= parameters.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(parameters.SearchName))
                Query.Where(p => p.Name.ToLower().Contains(parameters.SearchName.ToLower()));

            Query.Include(p => p.Metal!).Include(p => p.Category);
        }
    }
}

