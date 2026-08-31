namespace Osta.Core.Feature.Booking.Query.Result
{
    public record GetAllBookingsAsCustomerResult
    {
        public int BookingId { get; init; }
        public string CustomerEmail { get; init; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Street { get; set; }
        public DateTime BookingDate { get; set; }

        public string TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public string TechnicianEmail { get; set; }
        public List<Bookingservicerecord> bookingservicerecord { get; set; }

    }
}
