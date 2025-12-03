using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reviews.Application.Commands.ReviewCommands.CreateReview;
using Reviews.Application.Commands.ReviewCommands.DeleteReview;
using Reviews.Application.Commands.ReviewCommands.UpdateReview;
using Reviews.Application.DTOs.ReviewDTOs;
using Reviews.Application.Queries.ReviewQueries;
using Reviews.Domain.Exceptions;

namespace Reviews.API.Controllers
{
    public class ReviewsController : BaseController
    {
        private readonly IMapper _mapper;

        public ReviewsController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateReviewCommand
                {
                    ProductId = dto.ProductId,
                    Score = dto.Score,
                    Title = dto.Title,
                    Body = dto.Body
                };
                var review = await _mediator.Send(command, cancellationToken);
                var reviewDto = _mapper.Map<ReviewDto>(review);

                // ETag for optimistic concurrency
                AddETagHeader(review.UpdatedAt?.ToString() ?? review.CreatedAt.ToString());
                return CreatedResult(nameof(GetReviewById), new { id = review.Id }, reviewDto);
            }
            catch (ValidationException ex)
            {
                return BadRequestResult(ex.Message);
            }
            catch (DomainException ex)
            {
                return BadRequestResult(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviewById(string id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetReviewByIdQuery { Id = id };
                var review = await _mediator.Send(query, cancellationToken);
                var reviewDto = _mapper.Map<ReviewDto>(review);

                AddETagHeader(review.UpdatedAt?.ToString() ?? review.CreatedAt.ToString());
                return OkResult(reviewDto);
            }
            catch (NotFoundException)
            {
                return NotFoundResult($"Review with Id '{id}' not found");
            }
        }

        [HttpGet("product/{productId}")]
        [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductReviews(int productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetProductReviewsQuery
            {
                ProductId = productId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var reviews = await _mediator.Send(query, cancellationToken);
            var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);
            return OkResult(reviewDtos);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<ReviewDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchReviews([FromQuery] string searchText, [FromQuery] int? productId, [FromQuery] int? minRating,
            [FromQuery] int? maxRating, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = new SearchReviewsQuery
                {
                    SearchText = searchText,
                    ProductId = productId,
                    MinRating = minRating,
                    MaxRating = maxRating,
                    StartDate = startDate,
                    EndDate = endDate,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var reviews = await _mediator.Send(query, cancellationToken);
                var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews);
                return OkResult(reviewDtos);
            }
            catch (ValidationException ex)
            {
                return BadRequestResult(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var command = new UpdateReviewCommand
                {
                    Id = id,
                    Title = dto.Title,
                    Body = dto.Body
                };

                var review = await _mediator.Send(command, cancellationToken);
                var reviewDto = _mapper.Map<ReviewDto>(review);
                AddETagHeader(review.UpdatedAt?.ToString() ?? review.CreatedAt.ToString());
                return OkResult(reviewDto);
            }
            catch (NotFoundException)
            {
                return NotFoundResult($"Review with Id '{id}' not found");
            }
            catch (ValidationException ex)
            {
                return BadRequestResult(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(string id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteReviewCommand { Id = id };
                await _mediator.Send(command, cancellationToken);
                return NoContentResult();
            }
            catch (NotFoundException)
            {
                return NotFoundResult($"Review with Id '{id}' not found");
            }
        }

        [HttpGet("product/{productId}/stats")]
        [ProducesResponseType(typeof(ProductReviewStatsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductStats(int productId, CancellationToken cancellationToken)
        {
            var reviews = await _mediator.Send(new GetProductReviewsQuery { ProductId = productId }, cancellationToken);
            var stats = new ProductReviewStatsDto
            {
                ProductId = productId,
                TotalReviews = reviews.Count,
                AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating.Score.Value), 2) : 0.0
            };
            return OkResult(stats);
        }
    }
}

