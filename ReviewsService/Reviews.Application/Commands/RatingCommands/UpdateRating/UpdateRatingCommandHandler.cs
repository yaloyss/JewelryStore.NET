using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Exceptions;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Commands.RatingCommands.UpdateRating
{
    public class UpdateRatingCommandHandler : IRequestHandler<UpdateRatingCommand, Rating>
    {
        private readonly IRatingRepository _ratingRepository;

        public UpdateRatingCommandHandler(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<Rating> Handle(UpdateRatingCommand request, CancellationToken cancellationToken)
        {
            var rating = await _ratingRepository.GetByIdAsync(request.Id, cancellationToken);
            if (rating == null)
                throw new NotFoundException(request.Id);

            rating.UpdateScore(request.Score);
            return await _ratingRepository.UpdateAsync(rating, cancellationToken);
        }
    }
}

