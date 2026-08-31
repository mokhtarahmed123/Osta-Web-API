using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianImage;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianModel
{
    public record AddTechnicianCommand : IRequest<Response<string>>
    {

        public string? Bio { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public string NationalId { get; set; }

        public AddModelTechnicianImage Images { get; set; } = null!;
        public required List<int> ServiceAreas { get; set; }


    }
}
