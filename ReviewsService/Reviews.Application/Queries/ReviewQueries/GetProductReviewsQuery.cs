using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Queries.ReviewQueries
{
    public class GetProductReviewsQuery : IQuery<List<Review>>
    {
        public int ProductId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

