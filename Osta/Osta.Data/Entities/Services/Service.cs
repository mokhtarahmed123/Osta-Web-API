using Osta.Data.Entities.Technician;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Services
{
    [Table("Services", Schema = "Service")]

    public class Service
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;


        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }


        public Category Category { get; set; } = null!;



        public ICollection<TechnicianService> TechnicianServices { get; set; } = new List<TechnicianService>();
        public ICollection<BookingService> BookingService { get; set; } = new List<BookingService>();
    }
}