using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Queries.ReviewQueries
{
	public class SearchReviewsQuery : IQuery<List<Review>>
    {
        public string SearchText { get; set; }
        public int? ProductId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

