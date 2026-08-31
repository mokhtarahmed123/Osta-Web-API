using Osta.Core.Feature.MediaBooking.Command.Model;
using Osta.Data.Entities.Booking;

namespace Osta.Core.Mapping.MediaBookingMapping
{
    public partial class MediaBookingProfile
    {
        private void Update()
        {
            CreateMap<UpdateMediaBookingCommand, Media>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
                .ForMember(dest => dest.RepairMediaType, opt => opt.MapFrom(src => src.RepairMediaType))
                .ForMember(dest => dest.FileUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        }
    }
}
