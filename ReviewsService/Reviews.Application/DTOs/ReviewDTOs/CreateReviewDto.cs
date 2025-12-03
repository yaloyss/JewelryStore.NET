namespace Reviews.Application.DTOs.ReviewDTOs
{
	public class CreateReviewDto
	{
        public int ProductId { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}

