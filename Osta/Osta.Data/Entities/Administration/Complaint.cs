using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Osta.Data.Entities.Administration;


public class Complaint
{

    public int Id { get; set; }

    public int BookingId { get; set; }


    public Bookings Booking { get; set; } = null!;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


}
