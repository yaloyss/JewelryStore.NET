using AutoMapper;
using JewelryStore.OrdersService.Orders.BLL.DTOs;
using JewelryStore.OrdersService.Orders.BLL.Services.Interfaces;
using JewelryStore.OrdersService.Orders.DAL.UOW;
using JewelryStore.OrdersService.Orders.Domain.Exceptions;

namespace JewelryStore.OrdersService.Orders.BLL.Services
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

        public async Task<ProductDTO> GetProductByIdAsync(int productId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var product = await _unitOfWork.Products.GetByIdAsync(productId);
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

        public async Task<bool> IsProductAvailableAsync(int productId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var product = await _unitOfWork.Products.GetByIdAsync(productId);

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