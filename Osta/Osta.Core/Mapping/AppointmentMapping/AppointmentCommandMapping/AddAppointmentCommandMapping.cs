using Osta.Core.Feature.Appointment.Command.Model;
using Osta.Domain.Entities.Appointment;

namespace Osta.Core.Mapping.AppointmentMapping
{
    public partial class AppointmentProfile
    {
        private void Add()
        {
            CreateMap<AddAppointmentCommand, Appointment>()
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.ScheduledStart, opt => opt.MapFrom(src => src.ScheduledStart))
                .ForMember(dest => dest.ScheduledEnd, opt => opt.MapFrom(src => src.ScheduledEnd));
        }
    }
}
