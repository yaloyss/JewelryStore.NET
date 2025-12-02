using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Queries.ReviewQueries
{
	public class GetReviewByIdQuery : IQuery<Review>
    {
		public string Id { get; set; }
	}
}

