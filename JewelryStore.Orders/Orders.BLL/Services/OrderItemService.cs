using AutoMapper;
using JewelryStore.OrdersService.Orders.BLL.DTOs;
using JewelryStore.OrdersService.Orders.BLL.Services.Interfaces;
using JewelryStore.OrdersService.Orders.DAL.UOW;
using JewelryStore.OrdersService.Orders.Domain.Exceptions;

namespace JewelryStore.OrdersService.Orders.BLL.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderItemDTO> GetOrderItemByIdAsync(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var allOrders = await _unitOfWork.Orders.GetAllAsync();

                foreach (var order in allOrders)
                {
                    var orderItems = await _unitOfWork.OrderItems.GetByOrderIdAsync(order.OrderId);
                    var item = orderItems.FirstOrDefault(oi => oi.OrderItemId == id);

                    if (item != null)
                    {
                        await _unitOfWork.CommitAsync();
                        return _mapper.Map<OrderItemDTO>(item);
                    }
                }

                throw new NotFoundException($"OrderItem with id {id} not found");
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve order item: {ex.Message}");
            }
        }

        public async Task<IEnumerable<OrderItemDTO>> GetAllOrderItemsAsync()
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var allOrders = await _unitOfWork.Orders.GetAllAsync();
                var allOrderItems = new List<OrderItemDTO>();

                foreach (var order in allOrders)
                {
                    var orderItems = await _unitOfWork.OrderItems.GetByOrderIdAsync(order.OrderId);
                    var mappedItems = _mapper.Map<IEnumerable<OrderItemDTO>>(orderItems);
                    allOrderItems.AddRange(mappedItems);
                }

                await _unitOfWork.CommitAsync();
                return allOrderItems;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve order items: {ex.Message}");
            }
        }

        public async Task<IEnumerable<OrderItemDTO>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var items = await _unitOfWork.OrderItems.GetByOrderIdAsync(orderId);

                await _unitOfWork.CommitAsync();
                return _mapper.Map<IEnumerable<OrderItemDTO>>(items);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve order items for order {orderId}: {ex.Message}");
            }
        }
    }
}