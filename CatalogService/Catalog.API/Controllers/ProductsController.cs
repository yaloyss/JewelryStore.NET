using Catalog.BLL.DTOs.Product;
using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.Services.Interfaces;
using Catalog.DAL.Pagination;
using Catalog.Domain.Entities.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products with pagination, filtering and sorting
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<ProductDTO>>> GetAllProducts([FromQuery] ProductParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting products. Page: {Page}, Size: {Size}, OrderBy: {OrderBy}",parameters.PageNumber, parameters.PageSize, parameters.OrderBy);
            var result = await _productService.GetProductsPagedAsync(parameters, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a product by id
        /// </summary>
        /// <response code="400">Invalid ID.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", id);
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            return Ok(product);
        }

        /// <summary>
        /// Get detailed product information : metal, category, stones etc.
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ProductDetailedInfoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDetailedInfoDTO>> GetProductWithDetails(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting product details for ID: {ProductId}", id);
            var product = await _productService.GetProductWithDetailsAsync(id, cancellationToken);
            return Ok(product);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] CreateProductDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new product: {ProductName}", dto.Name);
            var product = await _productService.CreateProductAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);
            await _productService.DeleteProductAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Get products that contain stones with the specified names
        /// </summary>
        [HttpGet("by-stone-names")]
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByStoneNames([FromBody] List<string> stoneNames, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting products by stone names: {StoneNames}", string.Join(", ", stoneNames));
            var products = await _productService.GetProductsByStoneNamesAsync(stoneNames, cancellationToken);
            return Ok(products);
        }

        /// <summary>
        /// Get products that contain multiple stones
        /// </summary>
        [HttpGet("with-multiple-stones")]
        [ProducesResponseType(typeof(IEnumerable<ProductDetailedInfoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDetailedInfoDTO>>> GetProductsWithMultipleStones(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting products with multiple stones");
            var products = await _productService.GetProductsWithMultipleStonesAsync(cancellationToken);
            return Ok(products);
        }

        /// <summary>
        /// Get all stones of a product
        /// </summary>
        [HttpGet("{id}/stones-of-product")]
        [ProducesResponseType(typeof(IEnumerable<StoneDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<StoneDTO>>> GetProductStones(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting stones for product ID: {ProductId}", id);
            var stones = await _productService.GetProductStonesAsync(id, cancellationToken);
            return Ok(stones);
        }

        /// <summary>
        /// Add a stone to a product
        /// </summary>
        /// <response code="204">Stone successfully added.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="409">Stone already assigned to the product.</response>
        [HttpPost("{productId}/stones/{stoneId}/adding-stones-to-product")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddStoneToProduct(int productId, int stoneId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding stone {StoneId} to product {ProductId}", stoneId, productId);
            await _productService.AddStoneToProductAsync(productId, stoneId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Remove a stone from a product
        /// </summary>
        [HttpDelete("{productId}/stones/{stoneId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveStoneFromProduct(int productId, int stoneId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removing stone {StoneId} from product {ProductId}", stoneId, productId);
            await _productService.RemoveStoneFromProductAsync(productId, stoneId, cancellationToken);
            return NoContent();
        }
    }
}