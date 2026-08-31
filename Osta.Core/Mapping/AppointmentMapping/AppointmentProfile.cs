using AutoMapper;

namespace Osta.Core.Mapping.AppointmentMapping
{
    public partial class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            Add();
            Update();
            GetById();
            GetAll();
        }
    }
}
