using Osta.Core.Feature.Review.Query.Result;
using Osta.Data.Entities;

namespace Osta.Core.Mapping.ReviewMapping
{
    public partial class ReviewProfile
    {
        private void GetAllMyReviewsAsUser()
        {
            CreateMap<Review, GetAllMyReviewsAsUserResult>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
    .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));

        }
    }
}
