using Reviews.Application.DTOs.ReviewDTOs;

namespace Reviews.Application.DTOs.DiscussionDTO
{
	public class DiscussionWithReviewDto
	{
        public DiscussionDto Discussion { get; set; }
        public ReviewDto Review { get; set; }
    }
}

