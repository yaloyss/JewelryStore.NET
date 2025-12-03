using Catalog.BLL.DTOs.Metal;
using Catalog.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MetalsController : ControllerBase
    {
        private readonly IMetalService _metalService;
        private readonly ILogger<MetalsController> _logger;

        public MetalsController(IMetalService metalService, ILogger<MetalsController> logger)
        {
            _metalService = metalService;
            _logger = logger;
        }

        /// <summary>
        /// Get all metals
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MetalDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MetalDTO>>> GetAllMetals(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all metals");
            var metals = await _metalService.GetAllMetalsAsync(cancellationToken);
            return Ok(metals);
        }

        /// <summary>
        /// Get a metal by id
        /// </summary>
        /// <response code="200">Returns the metal</response>
        /// <response code="400">Invalid ID</response>
        /// <response code="404">Metal not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MetalDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MetalDTO>> GetMetalById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting metal with ID: {MetalId}", id);
            var metal = await _metalService.GetMetalByIdAsync(id, cancellationToken);
            return Ok(metal);
        }

        /// <summary>
        /// Get a metal by name
        /// </summary>
        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(MetalDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MetalDTO>> GetMetalByName(string name, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting metal with name: {MetalName}", name);
            var metal = await _metalService.GetMetalByNameAsync(name, cancellationToken);
            return Ok(metal);
        }

        /// <summary>
        /// Create a new metal
        /// </summary>
        /// <response code="201">Metal successfully created</response>
        [HttpPost]
        [ProducesResponseType(typeof(MetalDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MetalDTO>> CreateMetal([FromBody] CreateMetalDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new metal: {MetalName}", dto.Name);
            var metal = await _metalService.CreateMetalAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetMetalById), new { id = metal.MetalId }, metal);
        }

        /// <summary>
        /// Delete a metal
        /// </summary>
        /// <response code="204">Metal successfully deleted</response>
        /// <response code="409">Cannot delete a metal used in products.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteMetal(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting metal with ID: {MetalId}", id);
            await _metalService.DeleteMetalAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

