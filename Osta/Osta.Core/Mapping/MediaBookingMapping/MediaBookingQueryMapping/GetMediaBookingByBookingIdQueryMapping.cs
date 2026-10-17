using Osta.Core.Feature.MediaBooking.Query.Result;
using Osta.Data.Entities.Booking;

namespace Osta.Core.Mapping.MediaBookingMapping
{
    public partial class MediaBookingProfile
    {
        private void GetMediaBookingByBookingIdQuery()
        {
            CreateMap<Media, GetMediaBookingByBookingIdResult>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UploadedByUserId))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
                .ForMember(dest => dest.RepairMediaType, opt => opt.MapFrom(src => src.RepairMediaType))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.FileUrl))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        }
    }
}
