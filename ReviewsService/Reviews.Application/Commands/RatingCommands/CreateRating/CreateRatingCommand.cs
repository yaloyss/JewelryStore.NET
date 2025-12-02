using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Commands.RatingCommands.CreateRating
{
    public class CreateRatingCommand : ICommand<Rating>
    {
        public int Score { get; set; }
    }
}

