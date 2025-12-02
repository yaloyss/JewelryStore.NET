using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Queries.DiscussinQueries
{
	public class GetDiscussionByIdQuery : IQuery<Discussion>
    {
		public string Id { get; set; }
	}
}

