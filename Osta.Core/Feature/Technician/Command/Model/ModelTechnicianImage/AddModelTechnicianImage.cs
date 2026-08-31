using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Osta.Core.Feature.Technician.Command.Model.ModelTechnicianImage
{
    public class AddModelTechnicianImage
    {
        [Required(ErrorMessage = "Profile image is required.")]
        public IFormFile ProfileImage { get; set; } = null!;

        [Required(ErrorMessage = "Front national ID image is required.")]
        public IFormFile FrontNationalIdImage { get; set; } = null!;

        [Required(ErrorMessage = "Back national ID image is required.")]
        public IFormFile BackNationalIdImage { get; set; } = null!;
    }
}
