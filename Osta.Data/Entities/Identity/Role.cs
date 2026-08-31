using Microsoft.AspNetCore.Identity;
using Osta.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Identity
{
    [Table("Roles", Schema = "Identity")]

    public class Role : IdentityRole
    {

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public ICollection<RolePermission> Permissions { get; set; } = new HashSet<RolePermission>();

    }
}
