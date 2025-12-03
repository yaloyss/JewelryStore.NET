using Reviews.Application.DTOs.MessageDTO;

namespace Reviews.Application.DTOs.DiscussionDTO
{
	public class DiscussionDto
	{
        public string Id { get; set; }
        public string ReviewId { get; set; }
        public List<MessageDto> Messages { get; set; }
        public int MessageCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

