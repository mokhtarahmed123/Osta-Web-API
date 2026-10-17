using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Domain.Entities.Identity
{
    [Table("Permission", Schema = "Identity")]
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; }
        public string resource { get; set; } = string.Empty;
        public string action { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;

        public ICollection<RolePermission> Permissions { get; set; } = new HashSet<RolePermission>();

    }
}
