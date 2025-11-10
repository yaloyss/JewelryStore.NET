using AutoMapper;
using Orders.BLL.DTOs;
using Orders.BLL.Services.Interfaces;
using Orders.DAL.UOW;
using Orders.Domain.Exceptions;

namespace Orders.BLL.Services
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

        public async Task<ProductDTO> GetProductByIdAsync(int productId, CancellationToken ct = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var product = await _unitOfWork.Products.GetByIdAsync(productId, ct);
                if (product == null)
                {
                    throw new NotFoundException($"Product with ID {productId} not found");
                }

                await _unitOfWork.CommitAsync();
                return _mapper.Map<ProductDTO>(product);
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve product: {ex.Message}");
            }
        }

        public async Task<bool> IsProductAvailableAsync(int productId, CancellationToken ct = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var product = await _unitOfWork.Products.GetByIdAsync(productId, ct);

                await _unitOfWork.CommitAsync();

                return product != null;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to check product availability: {ex.Message}");
            }
        }
    }
}