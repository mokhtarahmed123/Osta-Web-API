using AutoMapper;

namespace Osta.Core.Mapping.ReviewMapping
{
    public partial class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            Add();
            Update();
            GetAllMyReviewsAsTechnician();
            GetReviewById();
            GetAllMyReviewsAsUser();
            GetAll();
        }

    }
}
