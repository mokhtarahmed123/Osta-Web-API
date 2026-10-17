using AutoMapper;

namespace Osta.Core.Mapping.MediaBookingMapping
{
    public partial class MediaBookingProfile : Profile
    {
        public MediaBookingProfile()
        {
            Add();
            Update();
            GetMediaBookingByBookingIdQuery();
            GetMediaBookingById();
            GetMediaBookingByTypeQuery();
        }
    }
}
