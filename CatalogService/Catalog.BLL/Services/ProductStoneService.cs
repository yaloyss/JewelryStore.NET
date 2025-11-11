using AutoMapper;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.ProductStone;
using Catalog.BLL.Exceptions;
using Catalog.BLL.Services.Interfaces;
using Catalog.DAL.UOW;

namespace Catalog.BLL.Services
{
    public class ProductStoneService : IProductStoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductStoneService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByStoneAsync(int stoneId, CancellationToken cancellationToken = default)
        {
            if (stoneId <= 0)
            {
                throw new ValidationException("StoneId must be greater than 0.");
            }

            var stoneExists = await _unitOfWork.Stones.GetByIdAsync(stoneId, cancellationToken);
            if (stoneExists == null)
            {
                throw new NotFoundException($"Stone with ID {stoneId} not found.");
            }

            var products = await _unitOfWork.ProductStones.GetProductsByStoneAsync(stoneId, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByStoneNamesAsync(FindProductsByStoneNamesDTO dto, CancellationToken cancellationToken = default)
        {
            if (dto.StoneNames == null || !dto.StoneNames.Any())
            {
                throw new ValidationException("Stone names list cannot be empty.");
            }

            //checking if every stone exists
            foreach (var stoneName in dto.StoneNames)
            {
                var stone = await _unitOfWork.Stones.GetStoneByNameAsync(stoneName, cancellationToken);
                if (stone == null)
                {
                    throw new NotFoundException($"Stone with name '{stoneName}' not found.");
                }
            }
            var products = await _unitOfWork.ProductStones.GetProductsByStoneNamesAsync(dto.StoneNames, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDetailedInfoDTO>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default)
        {
            var productStones = await _unitOfWork.ProductStones.GetProductsWithMultipleStonesAsync(cancellationToken);

            // GroupBy ProductId to get unique products
            var uniqueProducts = productStones.GroupBy(ps => ps.ProductId).Select(g => g.First().Product).ToList();
            return _mapper.Map<IEnumerable<ProductDetailedInfoDTO>>(uniqueProducts);
        }
    }
}

