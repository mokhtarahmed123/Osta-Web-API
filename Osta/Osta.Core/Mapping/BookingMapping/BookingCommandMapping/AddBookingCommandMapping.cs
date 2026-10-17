using Osta.Core.Feature.Booking.Command.Model.CustomerModel;
using Osta.Data.Entities.Booking;
using Osta.Data.Enum;

namespace Osta.Core.Mapping.Booking
{
    public partial class BookingProfile
    {
        private void Add()
        {
            CreateMap<AddBookingCommand, Bookings>()
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom((src, dest) => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom((src, dest) => BookingStatus.Pending))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
                .ForMember(dest => dest.TechnicianId, opt => opt.MapFrom(src => src.TechnicianId))
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore());
        }
    }
}
