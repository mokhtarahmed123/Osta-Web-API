using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Appointment;
using Osta.Infrastructure.Abstract.AppointmentAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.AppointmentImpelmention
{
    public class AppointmentRepository : GenericRepositoryAsync<Appointment>, IAppointmentRepository
    {
        #region Vars / Props
        private readonly DbSet<Appointment> Appointment;
        #endregion
        #region Constructor(s)
        public AppointmentRepository(OstaContext dbContext) : base(dbContext)
        {
            Appointment = dbContext.Set<Appointment>();
        }

        #endregion

    }
}
