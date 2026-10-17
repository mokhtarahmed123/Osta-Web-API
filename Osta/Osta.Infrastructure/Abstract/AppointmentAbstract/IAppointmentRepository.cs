using Osta.Domain.Entities.Appointment;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.AppointmentAbstract
{
    public interface IAppointmentRepository : IGenericRepositoryAsync<Appointment>
    {
    }
}
