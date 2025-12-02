using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.RatingCommands.UpdateRating
{
    public class UpdateRatingCommand : ICommand<Rating>
    {
        public string Id { get; set; }
        public int Score { get; set; }
    }
}

