namespace Osta.Core.Feature.Booking.Query.Result
{
    public record GetAllBookingsAsTechnicianResult
    {
        public int BookingId { get; init; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Street { get; set; }
        public DateTime BookingDate { get; set; }

        public List<Bookingservicerecord> bookingservicerecord { get; set; }


    }

}