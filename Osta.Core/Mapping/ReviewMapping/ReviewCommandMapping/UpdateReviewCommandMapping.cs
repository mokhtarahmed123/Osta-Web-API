using Osta.Core.Feature.Review.Command.Model;
using Osta.Data.Entities;

namespace Osta.Core.Mapping.ReviewMapping
{
    public partial class ReviewProfile
    {
        private void Update()
        {
            CreateMap<UpdateReviewCommand, Review>()
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        }
    }
}
