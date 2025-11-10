using Orders.BLL.DTOs;
using Orders.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Orders.API.Controllers
{
    //managing order items
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        private readonly ILogger<OrderItemController> _logger;

        public OrderItemController(IOrderItemService orderItemService, ILogger<OrderItemController> logger)
        {
            _orderItemService = orderItemService;
            _logger = logger;
        }

        /// <summary>
        /// Gets order item by id
        /// </summary>
        /// <response code="200">Order item found</response>
        /// <response code="404">Order item not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderItemDTO>> GetOrderItemById(int id, CancellationToken ct)
        {
            _logger.LogInformation("Retrieving order item {OrderItemId}", id);
            var orderItem = await _orderItemService.GetOrderItemByIdAsync(id, ct);
            return Ok(orderItem);
        }

        /// <summary>
        /// Gets all order items
        /// </summary>
        /// <returns>List of all order items</returns>
        /// <response code="200">Order items retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderItemDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetAllOrderItems(CancellationToken ct)
        {
            _logger.LogInformation("Retrieving all order items");
            var orderItems = await _orderItemService.GetAllOrderItemsAsync(ct);
            _logger.LogInformation("Retrieved {Count} order items", orderItems.Count());
            return Ok(orderItems);
        }

        /// <summary>
        /// Gets all order items for a specific order 
        /// </summary>
        /// <response code="200">Order items found</response>
        /// <response code="404">Order not found</response>
        [HttpGet("by-order/{orderId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<OrderItemDTO>>> GetOrderItemsByOrderId(int orderId, CancellationToken ct)
        {
            _logger.LogInformation("Retrieving order items for order {OrderId}", orderId);
            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId, ct);
            _logger.LogInformation("Found {Count} items for order {OrderId}", orderItems.Count(), orderId);
            return Ok(orderItems);
        }
    }
}

