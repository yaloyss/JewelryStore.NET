using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.ReviewCommands.CreateReview
{
    public class CreateReviewCommand : ICommand<Review>
    {
        public int ProductId { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}

