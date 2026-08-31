using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Identity
{
    [Table("RefreshTokens", Schema = "Identity")]

    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaxLength(200)]
        public string refreshToken { get; set; } = string.Empty;

        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}