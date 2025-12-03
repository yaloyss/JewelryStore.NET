using Reviews.Application.DTOs.RatingDTO;

namespace Reviews.Application.DTOs.ReviewDTOs
{
	public class ReviewDto
	{
        public string Id { get; set; }
        public int ProductId { get; set; }
        public RatingDto Rating { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

