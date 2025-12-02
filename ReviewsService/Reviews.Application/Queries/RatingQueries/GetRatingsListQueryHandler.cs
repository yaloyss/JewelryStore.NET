using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Queries.RatingQueries
{
    public class GetRatingsListQueryHandler : IRequestHandler<GetRatingsListQuery, List<Rating>>
    {
        private readonly IRatingRepository _ratingRepository;

        public GetRatingsListQueryHandler(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<List<Rating>> Handle(GetRatingsListQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Rating> ratings;

            if (request.FilterByScore.HasValue)
            {
                ratings = await _ratingRepository.GetByScoreAsync(request.FilterByScore.Value, cancellationToken);

                // pagination after filtering
                ratings = ratings
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }
            else
            {
                ratings = await _ratingRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
            }

            return ratings.ToList();
        }
    }
}

