using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Commands.RatingCommands.CreateRating
{
	public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, Rating>
    { 
        private readonly IRatingRepository _ratingRepository;

        public CreateRatingCommandHandler(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<Rating> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
        {
            var rating = new Rating(request.Score);
            return await _ratingRepository.AddAsync(rating, cancellationToken);
        }
    }
}

