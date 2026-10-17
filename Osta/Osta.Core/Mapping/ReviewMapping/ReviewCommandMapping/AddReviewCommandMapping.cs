using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities;

namespace Osta.Core.Mapping.ReviewMapping
{
    public partial class ReviewProfile
    {
        private void Add()
        {
            CreateMap<AddReviewCommand, Review>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        }
    }
}
