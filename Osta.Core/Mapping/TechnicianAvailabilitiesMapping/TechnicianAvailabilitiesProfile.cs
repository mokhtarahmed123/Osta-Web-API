using AutoMapper;

namespace Osta.Core.Mapping.TechnicianAvailabilitiesMapping
{
    public partial class TechnicianAvailabilitiesProfile : Profile
    {
        public TechnicianAvailabilitiesProfile()
        {
            Add();
            GetAll();
            GetAllByTechnicianId();
            GetById();
            Update();
        }
    }
}
