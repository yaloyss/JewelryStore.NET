using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reviews.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BaseController : ControllerBase
	{
        protected readonly IMediator _mediator;

        protected BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //200 OK with data
        protected IActionResult OkResult<T>(T data)
        {
            return Ok(data);
        }

        //201 
        protected IActionResult CreatedResult<T>(string actionName, object routeValues, T data)
        {
            return CreatedAtAction(actionName, routeValues, data);
        }

        //204
        protected IActionResult NoContentResult()
        {
            return NoContent();
        }

        //400 with errors
        protected IActionResult BadRequestResult(string message)
        {
            return BadRequest(new { error = message });
        }

        //404
        protected IActionResult NotFoundResult(string message = "Ресурс не знайдено")
        {
            return NotFound(new { error = message });
        }

        //409 for concurrency conflicts
        protected IActionResult ConflictResult(string message)
        {
            return Conflict(new { error = message });
        }

        //adds ETag header for optimistic concurrency
        protected void AddETagHeader(string etag)
        {
            Response.Headers.Add("ETag", $"\"{etag}\"");
        }

        // gets ETag from request header
        protected string GetIfMatchHeader()
        {
            return Request.Headers["If-Match"].ToString().Trim('"');
        }
    }
}

