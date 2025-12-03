using Catalog.BLL.DTOs.Category;
using Catalog.BLL.DTOs.Product;
using Catalog.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all categories");
            var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return Ok(categories);
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        /// <response code="400">invalid id</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting category with ID: {CategoryId}", id);
            var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
            return Ok(category);
        }

        /// <summary>
        /// Get category with products
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(CategoryWithInfoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryWithInfoDTO>> GetCategoryWithDetails(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting category details for ID: {CategoryId}", id);
            var category = await _categoryService.GetCategoryWithDetailsAsync(id, cancellationToken);
            return Ok(category);
        }

        /// <summary>
        /// Get quantity of products by metals
        /// </summary>
        [HttpGet("{id}/statistics")]
        [ProducesResponseType(typeof(CategoryStatisticsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryStatisticsDTO>> GetCategoryStatistics(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting statistics for category ID: {CategoryId}", id);
            var statistics = await _categoryService.GetCategoryStatisticsAsync(id, cancellationToken);
            return Ok(statistics);
        }

        /// <summary>
        /// Get all products of certain category
        /// </summary>
        [HttpGet("{id}/products")]
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsForCategory(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting products for category ID: {CategoryId}", id);
            var products = await _categoryService.GetProductsForCategoryAsync(id, cancellationToken);
            return Ok(products);
        }

        /// <summary>
        /// Get nymber of products in the category
        /// </summary>
        [HttpGet("{id}/products/count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetProductCount(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting product count for category ID: {CategoryId}", id);
            var count = await _categoryService.GetProductCountByCategoryAsync(id, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Create a category
        /// </summary>
        /// <response code="409">Category with this name already exists</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CategoryDTO>> CreateCategory([FromBody] CreateCategoryDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new category: {CategoryName}", dto.Name);
            var category = await _categoryService.CreateCategoryAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <response code="204">Deleted successfully</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
            await _categoryService.DeleteCategoryAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

