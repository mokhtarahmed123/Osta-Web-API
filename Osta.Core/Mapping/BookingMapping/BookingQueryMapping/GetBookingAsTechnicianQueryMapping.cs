using Osta.Core.Feature.Booking.Query.Result;
using Osta.Data.Entities.Booking;

namespace Osta.Core.Mapping.Booking
{
    public partial class BookingProfile
    {
        private void GetById()
        {
            CreateMap<Bookings, GetBookingAsTechnicianResult>()
        .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
        .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
        .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate))
        .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        }
    }
}
