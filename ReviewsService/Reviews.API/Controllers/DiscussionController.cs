using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reviews.Application.Commands.DiscussionCommand;
using Reviews.Application.DTOs.DiscussionDTO;
using Reviews.Application.Queries.DiscussinQueries;
using Reviews.Domain.Exceptions;

namespace Reviews.API.Controllers
{
    public class DiscussionsController : BaseController
    {
        private readonly IMapper _mapper;

        public DiscussionsController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(DiscussionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateDiscussion([FromBody] CreateDiscussionDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateDiscussionCommand
                {
                    ReviewId = dto.ReviewId,
                    InitialMessage = dto.InitialMessage
                };

                var discussion = await _mediator.Send(command, cancellationToken);
                var discussionDto = _mapper.Map<DiscussionDto>(discussion);
                AddETagHeader(discussion.UpdatedAt?.ToString() ?? discussion.CreatedAt.ToString());
                return CreatedResult(nameof(GetDiscussionById), new { id = discussion.Id }, discussionDto);
            }
            catch (ValidationException ex)
            {
                return BadRequestResult(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFoundResult(ex.Message);
            }
            catch (DomainException ex)
            {
                return BadRequestResult(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiscussionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDiscussionById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetDiscussionByIdQuery { Id = id };
                var discussion = await _mediator.Send(query, cancellationToken);
                var discussionDto = _mapper.Map<DiscussionDto>(discussion);
                AddETagHeader(discussion.UpdatedAt?.ToString() ?? discussion.CreatedAt.ToString());
                return OkResult(discussionDto);
            }
            catch (NotFoundException)
            {
                return NotFoundResult($"Discussion with Id '{id}' not found");
            }
        }

        [HttpPost("{id}/messages")]
        [ProducesResponseType(typeof(DiscussionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddMessage(string id, [FromBody] AddMessageDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var command = new AddMessageCommand
                {
                    DiscussionId = id,
                    MessageText = dto.MessageText
                };

                var discussion = await _mediator.Send(command, cancellationToken);
                var discussionDto = _mapper.Map<DiscussionDto>(discussion);
                AddETagHeader(discussion.UpdatedAt?.ToString() ?? discussion.CreatedAt.ToString());
                return OkResult(discussionDto);
            }
            catch (ValidationException ex)
            {
                return BadRequestResult(ex.Message);
            }
            catch (NotFoundException)
            {
                return NotFoundResult($"Discussion with Id '{id}' not found");
            }
            catch (DomainException ex)
            {
                return BadRequestResult(ex.Message);
            }
        }

        [HttpGet("{id}/with-review")]
        [ProducesResponseType(typeof(DiscussionWithReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDiscussionWithReview(string id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetDiscussionByIdQuery { Id = id };
                var discussion = await _mediator.Send(query, cancellationToken);

                //review through its id
                var reviewQuery = new Reviews.Application.Queries.ReviewQueries.GetReviewByIdQuery
                {
                    Id = discussion.ReviewId.ToString()
                };
                var review = await _mediator.Send(reviewQuery, cancellationToken);
                var result = _mapper.Map<DiscussionWithReviewDto>((discussion, review));
                return OkResult(result);
            }
            catch (NotFoundException)
            {
                return NotFoundResult($"Discussion with Id '{id}' not found");
            }
        }
    }
}

