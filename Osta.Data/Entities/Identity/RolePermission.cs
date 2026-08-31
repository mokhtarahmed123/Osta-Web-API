using Osta.Data.Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Identity
{
    [Table("RolePermission", Schema = "Identity")]

    public class RolePermission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RoleId { get; set; }
        public Guid PermissionId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; } = null!;
    }
}
