using Orders.BLL.Services.Interfaces;
using Orders.BLL.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <response code="201">Order successfully created</response>
        /// <response code="400">Invalid input data or validation error</response>
        /// <response code="404">Customer or Product not found</response>
        /// <response code="409">Business conflict occurred</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] OrderCreateDTO orderCreateDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items", orderCreateDto.CustomerId, orderCreateDto.Items.Count);
            var createdOrder = await _orderService.CreateOrderAsync(orderCreateDto, ct);
            _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId}", createdOrder.OrderId, createdOrder.CustomerId);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrder);
        }

        /// <summary>
        /// Gets an order by ID
        /// </summary>
        /// <returns>Order details with customer and items</returns>
        /// <response code="200">Order found and returned</response>
        /// <response code="404">Order not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id, CancellationToken ct)
        {
            _logger.LogInformation("Retrieving order {OrderId}", id);
            var order = await _orderService.GetOrderByIdAsync(id, ct);
            return Ok(order);
        }

        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <returns>List of all orders with summary information</returns>
        /// <response code="200">Orders retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderListDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetAllOrders(CancellationToken ct)
        {
            _logger.LogInformation("Retrieving all orders");
            var orders = await _orderService.GetAllOrdersAsync(ct);
            _logger.LogInformation("Retrieved {Count} orders", orders.Count());
            return Ok(orders);
        }

        /// <summary>
        /// Gets orders by customer name
        /// </summary>
        /// <returns>List of orders for matching customers</returns>
        /// <response code="200">Orders found for matching customers</response>
        /// <response code="400">Invalid search parameters (both names are empty)</response>
        /// <response code="404">No customers found with given name</response>
        [HttpGet("by-customer")]
        [ProducesResponseType(typeof(IEnumerable<OrderListDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderListDTO>>> GetOrdersByCustomerName([FromQuery] string? firstName, [FromQuery] string? lastName, CancellationToken ct)
        {
            _logger.LogInformation("Retrieving orders for customer: FirstName='{FirstName}', LastName='{LastName}'", firstName ?? "null", lastName ?? "null");
            var orders = await _orderService.GetOrdersByCustomerNameAsync(firstName, lastName, ct);
            _logger.LogInformation("Found {Count} orders for customer '{FirstName} {LastName}'", orders.Count(), firstName, lastName);
            return Ok(orders);
        }

        /// <summary>
        /// Updates order status
        /// </summary>
        /// <response code="200">Status updated successfully</response>
        /// <response code="400">Invalid status data</response>
        /// <response code="404">Order not found</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(OrderDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDTO>> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdateDTO statusUpdateDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating status for order {OrderId} to '{Status}'", id, statusUpdateDto.Status);
            var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, statusUpdateDto, ct);
            _logger.LogInformation("Order {OrderId} status updated successfully to '{Status}'", id, updatedOrder.Status);
            return Ok(updatedOrder);
        }

        /// <summary>
        /// Deletes an order
        /// </summary>
        /// <returns>No content on success</returns>
        /// <response code="204">Order deleted successfully</response>
        /// <response code="404">Order not found</response>
        /// <response code="409">Cannot delete order with current status (only Pending or Cancelled orders can be deleted)</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteOrder(int id, CancellationToken ct)
        {
            _logger.LogInformation("Attempting to delete order {OrderId}", id);
            await _orderService.DeleteOrderAsync(id, ct);
            _logger.LogInformation("Order {OrderId} deleted successfully", id);
            return NoContent();
        }
    }
}

