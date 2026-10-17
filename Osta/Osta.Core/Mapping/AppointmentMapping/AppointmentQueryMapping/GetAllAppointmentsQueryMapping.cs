using Osta.Core.Feature.Appointment.Query.Result;
using Osta.Domain.Entities.Appointment;

namespace Osta.Core.Mapping.AppointmentMapping
{
    public partial class AppointmentProfile
    {
        private void GetAll()
        {
            CreateMap<Appointment, GetAllAppointmentsResult>()
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.IsApproved))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.ScheduledStart, opt => opt.MapFrom(src => src.ScheduledStart))
                .ForMember(dest => dest.ScheduledEnd, opt => opt.MapFrom(src => src.ScheduledEnd));


        }
    }
}
