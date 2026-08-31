using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Services
{
    [Table("Services", Schema = "Service")]

    public class Service
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;



        public ICollection<TechnicianService> TechnicianServices { get; set; } = new List<TechnicianService>();
        public ICollection<BookingService> BookingService { get; set; } = new List<BookingService>();
    }
}