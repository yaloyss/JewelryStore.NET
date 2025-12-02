using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.ReviewCommands.UpdateReview
{
    public class UpdateReviewCommand : ICommand<Review>
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}

