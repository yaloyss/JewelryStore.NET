using Reviews.Application.Interfaces;

namespace Reviews.Application.Commands.ReviewCommands.DeleteReview
{
	public class DeleteReviewCommand : ICommand<bool>
	{
        public string Id { get; set; }
    }
}

