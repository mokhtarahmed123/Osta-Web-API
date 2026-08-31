using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Booking
{
    [Table("Media", Schema = "Booking")]
    public class Media
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Bookings Booking { get; set; } = null!;


        [MaxLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        public MediaFileType FileType { get; set; }

        public RepairMediaTypeEnum RepairMediaType { get; set; }

        public string UploadedByUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UploadedByUserId))]
        public User UploadedByUser { get; set; } = null!;

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
