using JewelryStore.OrdersService.Orders.BLL.DTOs;
using JewelryStore.OrdersService.Orders.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JewelryStore.OrdersService.Orders.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Gets customer by id
        /// </summary>
        /// <returns>Customer details</returns>
        /// <response code="200">Customer found</response>
        /// <response code="404">Customer not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDTO>> GetCustomerById(int id, CancellationToken ct)
        {
            _logger.LogInformation("Retrieving customer {CustomerId}", id);
            var customer = await _customerService.GetCustomerByIdAsync(id, ct);
            return Ok(customer);
        }

        /// <summary>
        /// Finds customer by name
        /// </summary>
        /// <param name="firstName">First name (optional, supports partial match)</param>
        /// <param name="lastName">Last name (optional, supports partial match)</param>
        /// <response code="200">Customers found</response>
        /// <response code="400">Invalid search parameters (both names are empty)</response>
        /// <response code="404">No customers found with given name</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> SearchCustomers([FromQuery] string? firstName, [FromQuery] string? lastName, CancellationToken ct)
        {
            _logger.LogInformation("Searching customers: FirstName='{FirstName}', LastName='{LastName}'", firstName ?? "null", lastName ?? "null");
            var customers = await _customerService.GetCustomersByNameAsync(firstName, lastName, ct);
            _logger.LogInformation("Found {Count} customers matching the search criteria", customers.Count());
            return Ok(customers);
        }

        /// <summary>
        /// Creates a customer
        /// </summary>
        /// <param name="customerDto">Customer data</param>
        /// <returns>Created customer ID</returns>
        /// <response code="201">Customer created successfully</response>
        /// <response code="400">Invalid customer data or validation error</response>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> CreateCustomer([FromBody] CustomerDTO customerDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating customer with email '{Email}'", customerDto.Email);
            var customerId = await _customerService.CreateCustomerAsync(customerDto, ct);
            _logger.LogInformation("Customer {CustomerId} created successfully", customerId);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customerId }, customerId);
        }

        /// <summary>
        /// Updates a customer
        /// </summary>
        /// <returns>No content on success</returns>
        /// <response code="204">Customer updated successfully</response>
        /// <response code="400">Invalid customer data, validation error, or id mismatch</response>
        /// <response code="404">Customer not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDTO customerDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerDto.CustomerId)
            {
                _logger.LogWarning("ID mismatch: route ID {RouteId} != body ID {BodyId}", id, customerDto.CustomerId);
                return BadRequest("ID mismatch between route and body");
            }

            _logger.LogInformation("Updating customer {CustomerId}", id);
            await _customerService.UpdateCustomerAsync(customerDto, ct);
            _logger.LogInformation("Customer {CustomerId} updated successfully", id);
            return NoContent(); 
        }

        /// <summary>
        /// Deletes customer
        /// </summary>
        /// <response code="204">Customer deleted successfully</response>
        /// <response code="404">Customer not found</response>
        /// <response code="409">Cannot delete customer with existing orders</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCustomer(int id, CancellationToken ct)
        {
            _logger.LogInformation("Attempting to delete customer {CustomerId}", id);
            await _customerService.DeleteCustomerAsync(id, ct);
            _logger.LogInformation("Customer {CustomerId} deleted successfully", id);
            return NoContent();
        }
    }
}

