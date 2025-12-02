using Reviews.Application.Interfaces;
using Reviews.Domain.Entities;

namespace Reviews.Application.Queries.RatingQueries
{
    public class GetRatingsListQuery : IQuery<List<Rating>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? FilterByScore { get; set; }
    }
}

