using AutoMapper;
using JewelryStore.OrdersService.Orders.Application.Services.Interfaces;
using JewelryStore.OrdersService.Orders.BLL.DTOs;
using JewelryStore.OrdersService.Orders.DAL.UOW;
using JewelryStore.OrdersService.Orders.Domain.Entities;
using JewelryStore.OrdersService.Orders.Domain.Exceptions;

namespace JewelryStore.OrdersService.Orders.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var customer = await _unitOfWork.Customers.GetByIdAsync(orderCreateDto.CustomerId);
                if (customer == null)
                {
                    throw new NotFoundException($"Customer with id {orderCreateDto.CustomerId} was not found");
                }

                if (orderCreateDto.Items == null || !orderCreateDto.Items.Any())
                {
                    throw new ValidationException("Order must contain at least one item");
                }

                foreach (var itemCreateDto in orderCreateDto.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(itemCreateDto.ProductId);
                    if (product == null)
                    {
                        throw new NotFoundException($"Product with id {itemCreateDto.ProductId} was not found");
                    }

                    if (itemCreateDto.Quantity <= 0)
                    {
                        throw new ValidationException("Quantity must be greater than 0");
                    }
                }

                var order = _mapper.Map<Order>(orderCreateDto);
                order.OrderDate = DateTime.UtcNow;
                order.Status = string.IsNullOrWhiteSpace(orderCreateDto.Status) ? "Pending" : orderCreateDto.Status;

                int orderId = await _unitOfWork.Orders.CreateAsync(order);

                await _unitOfWork.CommitAsync();
                return await GetOrderByIdAsync(orderId);
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (ValidationException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (BusinessConflictException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to create order: {ex.Message}");
            }
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new NotFoundException($"Order with id {orderId} was not found");
                }

                var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
                if (customer != null)
                {
                    order.Customer = customer;
                }

                var orderItems = await _unitOfWork.OrderItems.GetByOrderIdAsync(orderId);
                order.Items = orderItems.ToList();

                await _unitOfWork.CommitAsync();

                var orderDto = _mapper.Map<OrderDTO>(order);
                return orderDto;
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve order: {ex.Message}");
            }
        }

        public async Task<IEnumerable<OrderListDTO>> GetAllOrdersAsync()
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var orders = await _unitOfWork.Orders.GetAllAsync();
                var orderListDtos = new List<OrderListDTO>();

                foreach (var order in orders)
                {
                    var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
                    if (customer != null)
                    {
                        order.Customer = customer;
                    }

                    var items = await _unitOfWork.OrderItems.GetByOrderIdAsync(order.OrderId);
                    order.Items = items.ToList();

                    var orderListDto = _mapper.Map<OrderListDTO>(order);
                    orderListDtos.Add(orderListDto);
                }

                await _unitOfWork.CommitAsync();
                return orderListDtos;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve orders: {ex.Message}");
            }
        }

        public async Task<IEnumerable<OrderListDTO>> GetOrdersByCustomerNameAsync(string firstName, string lastName)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                {
                    throw new ValidationException("First name or last name must be provided");
                }

                var customers = await _unitOfWork.Customers.GetByNameAsync(firstName, lastName);
                if (customers == null || !customers.Any())
                {
                    throw new NotFoundException($"Customer with name '{firstName} {lastName}' was not found");
                }

                var orderListDtos = new List<OrderListDTO>();

                foreach (var customer in customers)
                {
                    var orders = await _unitOfWork.Orders.GetByCustomerIdAsync(customer.CustomerId);

                    foreach (var order in orders)
                    {
                        order.Customer = customer;

                        var items = await _unitOfWork.OrderItems.GetByOrderIdAsync(order.OrderId);
                        order.Items = items.ToList();

                        var orderListDto = _mapper.Map<OrderListDTO>(order);
                        orderListDtos.Add(orderListDto);
                    }
                }

                await _unitOfWork.CommitAsync();
                return orderListDtos;
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (ValidationException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve orders by customer name: {ex.Message}");
            }
        }

        public async Task<OrderDTO> UpdateOrderStatusAsync(int orderId, OrderStatusUpdateDTO statusUpdateDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new NotFoundException($"Order with id {orderId} was not found");
                }

                if (string.IsNullOrWhiteSpace(statusUpdateDto.Status))
                {
                    throw new ValidationException("Status cannot be empty");
                }

                order.Status = statusUpdateDto.Status;
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.CommitAsync();
                return await GetOrderByIdAsync(orderId);
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (ValidationException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (BusinessConflictException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to update order status: {ex.Message}");
            }
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    throw new NotFoundException($"Order with id {orderId} was not found");
                }

                // business rule -- can only delete pending/cancelled orders
                if (order.Status == "Processing" || order.Status == "Completed")
                {
                    throw new BusinessConflictException($"Cannot delete order with status '{order.Status}'. Only Pending or Cancelled orders can be deleted.");
                }

                await _unitOfWork.Orders.DeleteAsync(orderId);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (BusinessConflictException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to delete order: {ex.Message}");
            }
        }
    }
}