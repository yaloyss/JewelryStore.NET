namespace Reviews.Application.DTOs.ReviewDTOs
{
	public class ProductReviewStatsDto
	{
        public int ProductId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}

