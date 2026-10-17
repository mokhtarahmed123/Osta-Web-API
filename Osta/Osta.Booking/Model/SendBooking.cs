namespace Osta.Booking.Model
{
    public class SendBooking
    {

        public required string CustomerId { get; set; }

        public required string CustomerName { get; set; }

        public required string TechnicianId { get; set; }

        public int ServiceId { get; set; }
        public DateTime BookingDate { get; set; }


        public string BookingStatus { get; set; } = "Pending";

        public required string Area { get; set; }
        public required string City { get; set; }
        public required string Governorate { get; set; }
        public required string Street { get; set; }



    }
}
