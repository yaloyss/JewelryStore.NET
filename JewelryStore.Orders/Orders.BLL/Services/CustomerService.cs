using AutoMapper;
using JewelryStore.OrdersService.Orders.BLL.DTOs;
using JewelryStore.OrdersService.Orders.BLL.Services.Interfaces;
using JewelryStore.OrdersService.Orders.DAL.UOW;
using JewelryStore.OrdersService.Orders.Domain.Entities;
using JewelryStore.OrdersService.Orders.Domain.Exceptions;

namespace JewelryStore.OrdersService.Orders.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
                if (customer == null)
                {
                    throw new NotFoundException($"Customer with ID {customerId} not found");
                }

                await _unitOfWork.CommitAsync();
                return _mapper.Map<CustomerDTO>(customer);
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to retrieve customer: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CustomerDTO>> GetCustomersByNameAsync(string? firstName, string? lastName)
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
                    throw new NotFoundException($"No customers found with name '{firstName} {lastName}'");
                }

                await _unitOfWork.CommitAsync();
                return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
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
                throw new BusinessConflictException($"Failed to retrieve customers by name: {ex.Message}");
            }
        }

        public async Task<int> CreateCustomerAsync(CustomerDTO customerDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                ValidateCustomer(customerDto);

                // Маппінг DTO -> Entity
                var customer = _mapper.Map<Customer>(customerDto);

                // Створення клієнта
                var customerId = await _unitOfWork.Customers.CreateAsync(customer);

                await _unitOfWork.CommitAsync();
                return customerId;
            }
            catch (ValidationException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to create customer: {ex.Message}");
            }
        }

        public async Task<bool> UpdateCustomerAsync(CustomerDTO customerDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                ValidateCustomer(customerDto);

                var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(customerDto.CustomerId);
                if (existingCustomer == null)
                {
                    throw new NotFoundException($"Customer with ID {customerDto.CustomerId} not found");
                }

                var customer = _mapper.Map<Customer>(customerDto);

                var result = await _unitOfWork.Customers.UpdateAsync(customer);

                await _unitOfWork.CommitAsync();
                return result;
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
                throw new BusinessConflictException($"Failed to update customer: {ex.Message}");
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
                if (customer == null)
                {
                    throw new NotFoundException($"Customer with ID {customerId} not found");
                }

                var result = await _unitOfWork.Customers.DeleteAsync(customerId);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (NotFoundException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new BusinessConflictException($"Failed to delete customer: {ex.Message}");
            }
        }

        private void ValidateCustomer(CustomerDTO customerDto)
        {
            if (customerDto == null)
            {
                throw new ValidationException("Customer data is required");
            }

            if (string.IsNullOrWhiteSpace(customerDto.FirstName))
            {
                throw new ValidationException("First name is required");
            }

            if (string.IsNullOrWhiteSpace(customerDto.LastName))
            {
                throw new ValidationException("Last name is required");
            }

            if (string.IsNullOrWhiteSpace(customerDto.Email))
            {
                throw new ValidationException("Email is required");
            }

            if (!IsValidEmail(customerDto.Email))
            {
                throw new ValidationException("Invalid email format");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}