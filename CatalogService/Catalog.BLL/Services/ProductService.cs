using AutoMapper;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using Catalog.DAL.UOW;
using Catalog.Domain.Entities;

namespace Catalog.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                throw new ValidationException("ProductId must be greater than 0.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {productId} not found.");
            }

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDetailedInfoDTO> GetProductWithDetailsAsync(int productId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                throw new ValidationException("ProductId must be greater than 0.");
            }

            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {productId} not found.");
            }

            return _mapper.Map<ProductDetailedInfoDTO>(product);
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO dto, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {dto.CategoryId} not found.");
            }

            if (dto.MetalId.HasValue)
            {
                var metal = await _unitOfWork.Metals.GetByIdAsync(dto.MetalId.Value, cancellationToken);
                if (metal == null)
                {
                    throw new NotFoundException($"Metal with ID {dto.MetalId.Value} not found.");
                }
            }

            if (dto.StoneIds != null && dto.StoneIds.Any())
            {
                foreach (var stoneId in dto.StoneIds)
                {
                    var stone = await _unitOfWork.Stones.GetByIdAsync(stoneId, cancellationToken);
                    if (stone == null)
                    {
                        throw new NotFoundException($"Stone with ID {stoneId} not found.");
                    }
                }
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                var product = _mapper.Map<Product>(dto);
                await _unitOfWork.Products.AddAsync(product, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (dto.StoneIds != null && dto.StoneIds.Any())
                {
                    foreach (var stoneId in dto.StoneIds)
                    {
                        await _unitOfWork.ProductStones.AddStoneToProductAsync(product.ProductId, stoneId, cancellationToken);
                    }
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new Exception("An error occurred while creating the product.", ex);
            }
        }

        public async Task DeleteProductAsync(int productId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                throw new ValidationException("ProductId must be greater than 0.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {productId} not found.");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                //first deleting all relations with stones
                var stones = await _unitOfWork.ProductStones.GetProductStonesAsync(productId, cancellationToken);
                foreach (var stone in stones)
                {
                    await _unitOfWork.ProductStones.RemoveStoneFromProductAsync(productId, stone.StoneId, cancellationToken);
                }

                //then deleting a product
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new Exception("An error occurred while deleting the product.", ex);
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                throw new ValidationException("CategoryId must be greater than 0.");
            }

            var categoryExists = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (categoryExists == null)
            {
                throw new NotFoundException($"Category with ID {categoryId} not found.");
            }

            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByMetalAsync(int metalId, CancellationToken cancellationToken = default)
        {
            if (metalId <= 0)
            {
                throw new ValidationException("MetalId must be greater than 0.");
            }

            var metalExists = await _unitOfWork.Metals.GetByIdAsync(metalId, cancellationToken);
            if (metalExists == null)
            {
                throw new NotFoundException($"Metal with ID {metalId} not found.");
            }

            var products = await _unitOfWork.Products.GetProductsByMetalAsync(metalId, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsWithPriceRangeAsync(ProductPriceRangeDTO priceRange, CancellationToken cancellationToken = default)
        {
            if (priceRange.MinPrice > priceRange.MaxPrice)
            {
                throw new ValidationException("MinPrice cannot be greater than MaxPrice.");
            }

            var products = await _unitOfWork.Products.GetProductsWithPriceRangeAsync(priceRange.MinPrice, priceRange.MaxPrice, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByStoneNamesAsync(List<string> stoneNames, CancellationToken cancellationToken = default)
        {
            foreach (var stoneName in stoneNames)
            {
                var stone = await _unitOfWork.Stones.GetStoneByNameAsync(stoneName, cancellationToken);
                if (stone == null)
                {
                    throw new NotFoundException($"Stone with name '{stoneName}' not found.");
                }
            }
            var products = await _unitOfWork.ProductStones.GetProductsByStoneNamesAsync(stoneNames, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDetailedInfoDTO>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default)
        {
            var productStones = await _unitOfWork.ProductStones.GetProductsWithMultipleStonesAsync(cancellationToken);
            //grouping to get unique products
            var uniqueProducts = productStones.GroupBy(ps => ps.ProductId).Select(g => g.First().Product).ToList();
            return _mapper.Map<IEnumerable<ProductDetailedInfoDTO>>(uniqueProducts);
        }

        public async Task<IEnumerable<StoneDTO>> GetProductStonesAsync(int productId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                throw new ValidationException("ProductId must be greater than 0.");
            }
            var productExists = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (productExists == null)
            {
                throw new NotFoundException($"Product with ID {productId} not found.");
            }
            var stones = await _unitOfWork.ProductStones.GetProductStonesAsync(productId, cancellationToken);
            return _mapper.Map<IEnumerable<StoneDTO>>(stones);
        }

        public async Task AddStoneToProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                throw new ValidationException("ProductId must be greater than 0.");
            }

            if (stoneId <= 0)
            {
                throw new ValidationException("StoneId must be greater than 0.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {productId} not found.");
            }

            var stone = await _unitOfWork.Stones.GetByIdAsync(stoneId, cancellationToken);
            if (stone == null)
            {
                throw new NotFoundException($"Stone with ID {stoneId} not found.");
            }

            try
            {
                var result = await _unitOfWork.ProductStones.AddStoneToProductAsync(productId, stoneId, cancellationToken);
                if (!result)
                {
                    throw new BusinessConflictException($"Stone '{stone.Name}' is already added to this product.");
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (BusinessConflictException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding stone to product.", ex);
            }
        }

        public async Task RemoveStoneFromProductAsync(int productId, int stoneId, CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                throw new ValidationException("ProductId must be greater than 0.");
            }

            if (stoneId <= 0)
            {
                throw new ValidationException("StoneId must be greater than 0.");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {productId} not found.");
            }

            try
            {
                var result = await _unitOfWork.ProductStones.RemoveStoneFromProductAsync(productId, stoneId, cancellationToken);
                if (!result)
                {
                    throw new NotFoundException($"Stone with ID {stoneId} is not associated with product ID {productId}.");
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing stone from product.", ex);
            }
        }

    }
}