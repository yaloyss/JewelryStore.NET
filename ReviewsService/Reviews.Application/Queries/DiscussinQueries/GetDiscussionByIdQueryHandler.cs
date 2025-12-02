using MediatR;
using Reviews.Domain.Entities;
using Reviews.Domain.Exceptions;
using Reviews.Domain.Interfaces;

namespace Reviews.Application.Queries.DiscussinQueries
{
    public class GetDiscussionByIdQueryHandler : IRequestHandler<GetDiscussionByIdQuery, Discussion>
    {
        private readonly IDiscussionRepository _discussionRepository;

        public GetDiscussionByIdQueryHandler(IDiscussionRepository discussionRepository)
        {
            _discussionRepository = discussionRepository;
        }

        public async Task<Discussion> Handle(GetDiscussionByIdQuery request, CancellationToken cancellationToken)
        {
            var discussion = await _discussionRepository.GetByIdAsync(request.Id,cancellationToken);

            if (discussion == null)
                throw new NotFoundException(request.Id);
            return discussion;
        }
    }
}

