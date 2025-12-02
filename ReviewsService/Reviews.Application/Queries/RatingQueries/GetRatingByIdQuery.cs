using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Queries.RatingQueries
{
	public class GetRatingByIdQuery : IQuery<Rating>
	{
		public string Id { get; set; }
	}
}

