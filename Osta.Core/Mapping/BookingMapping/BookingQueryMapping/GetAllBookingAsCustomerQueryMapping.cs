using Osta.Booking.Model;
using Osta.Core.Feature.Booking.Query.Result;

namespace Osta.Core.Mapping.Booking
{
    public partial class BookingProfile
    {
        private void GetAllBookingAsCustomerQueryMapping()
        {
            CreateMap<GetAllBookingsAsCustomerdto, GetAllBookingsAsCustomerResult>();

        }
    }
}
