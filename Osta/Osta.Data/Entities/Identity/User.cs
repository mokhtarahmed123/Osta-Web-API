using Microsoft.AspNetCore.Identity;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations.Schema;
namespace Osta.Data.Entities.Identity
{
    [Table("AspNetUsers", Schema = "Identity")]
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public string Code { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public DateOnly? DateOfBirth { get; set; } = null;
        public string? Provider { get; set; }
        public string? ExternalId { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<Technicians> Technicians { get; set; } = new List<Technicians>();

        public ICollection<Media> Media { get; set; } = new List<Media>();
    }
}
