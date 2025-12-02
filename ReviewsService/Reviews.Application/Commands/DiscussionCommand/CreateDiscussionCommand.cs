using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.DiscussionCommand
{
    public class CreateDiscussionCommand : ICommand<Discussion>
    {
        public string ReviewId { get; set; }
        public string InitialMessage { get; set; }
    }
}

