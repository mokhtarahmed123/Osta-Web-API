using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Command.Model.ModelTechnicianImage;

namespace Osta.Core.Feature.Technician.Command.Model.TechnicianModel
{
    public record UpdateTechnicianCommand() : IRequest<Response<string>>
    {
        public string? Bio { get; set; } = string.Empty;
        public List<int>? ServiceAreas { get; set; }
        public string NationalId { get; set; }

        public UpdateModelTechnicianImage? Images { get; set; } = null!;

        public int YearsOfExperience { get; set; }

    }
}
