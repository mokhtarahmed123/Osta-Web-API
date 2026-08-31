using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Services
{
    [Table("Category", Schema = "Service")]

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}