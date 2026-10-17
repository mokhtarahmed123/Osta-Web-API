using AutoMapper;

namespace Osta.Core.Mapping.Booking
{
    public partial class BookingProfile : Profile
    {
        public BookingProfile()
        {
            Add();
            GetAllBookingAsTechnician();
            GetById();
            GetAllBookingAsCustomerQueryMapping();
        }
    }
}
