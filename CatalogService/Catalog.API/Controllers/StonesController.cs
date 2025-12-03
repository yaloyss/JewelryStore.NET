using Catalog.BLL.DTOs.Stone;
using Catalog.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StonesController : ControllerBase
    {
        private readonly IStoneService _stoneService;
        private readonly ILogger<StonesController> _logger;

        public StonesController(IStoneService stoneService, ILogger<StonesController> logger)
        {
            _stoneService = stoneService;
            _logger = logger;
        }

        /// <summary>
        /// Get all stones
        /// </summary>
        /// <response code="200">Returns the list of stones.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StoneDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StoneDTO>>> GetAllStones(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all stones");
            var stones = await _stoneService.GetAllStonesAsync(cancellationToken);
            return Ok(stones);
        }

        /// <summary>
        /// Get a stone by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StoneDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoneDTO>> GetStoneById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting stone with ID: {StoneId}", id);
            var stone = await _stoneService.GetStoneByIdAsync(id, cancellationToken);
            return Ok(stone);
        }

        /// <summary>
        /// Get a stone by name
        /// </summary>
        /// <response code="400">Invalid name.</response>
        /// <response code="404">Stone not found.</response>
        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(StoneDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StoneDTO>> GetStoneByName(string name, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting stone with name: {StoneName}", name);
            var stone = await _stoneService.GetStoneByNameAsync(name, cancellationToken);
            return Ok(stone);
        }

        /// <summary>
        /// Create a new stone
        /// </summary>
        /// <response code="409">Stone with the same name already exists.</response>
        [HttpPost]
        [ProducesResponseType(typeof(StoneDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<StoneDTO>> CreateStone([FromBody] CreateStoneDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new stone: {StoneName}", dto.Name);
            var stone = await _stoneService.CreateStoneAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetStoneById), new { id = stone.StoneId }, stone);
        }

        /// <summary>
        /// Delete a stone
        /// </summary>
        /// <response code="204">Stone successfully deleted.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteStone(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting stone with ID: {StoneId}", id);
            await _stoneService.DeleteStoneAsync(id, cancellationToken);
            return NoContent();
        }
    }
}

