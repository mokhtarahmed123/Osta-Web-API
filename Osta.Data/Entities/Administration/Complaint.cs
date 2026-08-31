using Osta.Data.Entities.Booking;
using Osta.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Administration;

[Table("Complaints", Schema = "Administration")]
public class Complaint
{
    [Key]
    public int Id { get; set; }

    public int BookingId { get; set; }

    [ForeignKey(nameof(BookingId))]
    public Bookings Booking { get; set; } = null!;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


}
