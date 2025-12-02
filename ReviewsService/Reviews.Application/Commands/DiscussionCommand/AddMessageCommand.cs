using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.DiscussionCommand
{
    public class AddMessageCommand : ICommand<Discussion>
    {
        public string DiscussionId { get; set; }
        public string MessageText { get; set; }
    }
}

