namespace Osta.Booking.Model
{
    public class GetAllBookingsAsCustomerdto
    {
        public int BookingId { get; set; }

        public required string TechnicianId { get; set; }
        public required string TechnicianName { get; set; }
        public required string TechnicianEmail { get; set; }

        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }

        public required string Status { get; set; }

        public required string Area { get; set; }
        public required string City { get; set; }
        public required string Governorate { get; set; }
        public required string Street { get; set; }

        public DateTime BookingDate { get; set; }
    }
}
