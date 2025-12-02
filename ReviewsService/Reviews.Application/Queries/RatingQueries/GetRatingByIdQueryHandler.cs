using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Queries.RatingQueries
{
    public class GetRatingByIdQueryHandler : IRequestHandler<GetRatingByIdQuery, Rating>
    {
        private readonly IRatingRepository _ratingRepository;

        public GetRatingByIdQueryHandler(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<Rating> Handle(GetRatingByIdQuery request, CancellationToken cancellationToken)
        {
            var rating = await _ratingRepository.GetByIdAsync(request.Id, cancellationToken);
            return rating;
        }
    }
}

