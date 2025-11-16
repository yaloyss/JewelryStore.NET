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


        public async Task<IEnumerable<ProductDetailedInfoDTO>> GetProductsWithMultipleStonesAsync(CancellationToken cancellationToken = default)
        {
            var productStones = await _unitOfWork.ProductStones.GetProductsWithMultipleStonesAsync(cancellationToken);

            // GroupBy ProductId to get unique products
            var uniqueProducts = productStones.GroupBy(ps => ps.ProductId).Select(g => g.First().Product).ToList();
            return _mapper.Map<IEnumerable<ProductDetailedInfoDTO>>(uniqueProducts);
        }
    }
}

