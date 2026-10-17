using Osta.Data.Entities.Technician;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations.Schema;

namespace Osta.Data.Entities.Services
{
    [Table("Services", Schema = "Service")]

    public class Service
    {

=======

namespace Osta.Data.Entities.Services
{
    public class Service
    {
>>>>>>> c73a0b6 (Initial commit)
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

<<<<<<< HEAD

=======
>>>>>>> c73a0b6 (Initial commit)
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }

<<<<<<< HEAD

        public Category Category { get; set; } = null!;



=======
        public Category Category { get; set; } = null!;

>>>>>>> c73a0b6 (Initial commit)
        public ICollection<TechnicianService> TechnicianServices { get; set; } = new List<TechnicianService>();
        public ICollection<BookingService> BookingService { get; set; } = new List<BookingService>();
    }
}