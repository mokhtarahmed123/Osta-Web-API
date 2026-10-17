using Osta.Data.Entities.Identity;
using Osta.Data.Enum;
using Osta.Domain.Enum;

namespace Osta.Data.Entities.Booking
{

    public class Media
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
<<<<<<< HEAD

        public Bookings Booking { get; set; } = null!;


=======
        public Bookings Booking { get; set; } = null!;
>>>>>>> c73a0b6 (Initial commit)
        public string FileUrl { get; set; } = string.Empty;

        public MediaFileType FileType { get; set; }

        public RepairMediaTypeEnum RepairMediaType { get; set; }

        public string UploadedByUserId { get; set; } = string.Empty;

        public User UploadedByUser { get; set; } = null!;

<<<<<<< HEAD

=======
>>>>>>> c73a0b6 (Initial commit)
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
