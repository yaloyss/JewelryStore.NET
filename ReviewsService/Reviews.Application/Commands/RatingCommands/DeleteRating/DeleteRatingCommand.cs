using Reviews.Application.Interfaces;

namespace Reviews.Application.Commands.RatingCommands.DeleteRating
{
	public class DeleteRatingCommand : ICommand<bool>
	{
        public string Id { get; set; }
    }
}

