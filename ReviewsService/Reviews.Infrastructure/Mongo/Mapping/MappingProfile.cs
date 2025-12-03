using AutoMapper;
using Reviews.Application.DTOs.DiscussionDTO;
using Reviews.Application.DTOs.MessageDTO;
using Reviews.Application.DTOs.RatingDTO;
using Reviews.Application.DTOs.ReviewDTOs;
using Reviews.Domain.Entities;
using Reviews.Domain.ValueObjects;

namespace Reviews.Infrastructure.Mongo.Mapping
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {
            CreateMap<Review, ReviewDto>().ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));

            CreateMap<CreateReviewDto, Review>()
                .ConstructUsing(src => new Review(src.ProductId, new Rating(src.Score), src.Title, src.Body));

            CreateMap<Rating, RatingDto>().ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score.Value));

            CreateMap<Discussion, DiscussionDto>()
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.ReviewId.ToString()))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.MessageCount, opt => opt.MapFrom(src => src.GetMessagesCount()));

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // discussion w review mapping
            CreateMap<(Discussion discussion, Review review), DiscussionWithReviewDto>()
                .ForMember(dest => dest.Discussion, opt => opt.MapFrom(src => src.discussion))
                .ForMember(dest => dest.Review, opt => opt.MapFrom(src => src.review));
        }
    }
}

