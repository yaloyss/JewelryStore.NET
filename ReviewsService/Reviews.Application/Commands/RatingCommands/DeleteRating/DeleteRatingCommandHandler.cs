using MediatR;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Commands.RatingCommands.DeleteRating
{
    public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, bool>
    {
        private readonly IRatingRepository _ratingRepository;

        public DeleteRatingCommandHandler(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<bool> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            return await _ratingRepository.DeleteAsync(request.Id, cancellationToken);
        }
    }
}

