using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Technician.Command.Model.ModelTechnicianAvailabilities
{
    public record UpdateTechnicianAvailabilityCommand(int Id) : IRequest<Response<string>>
    {
        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;


    }
}
