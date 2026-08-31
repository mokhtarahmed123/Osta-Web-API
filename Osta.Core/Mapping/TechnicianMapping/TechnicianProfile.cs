using AutoMapper;

namespace Osta.Core.Mapping.TechnicianMapping
{
    public partial class TechnicianProfile : Profile
    {
        public TechnicianProfile()
        {
            Add();
            GetAllTechnicians();
            GetAllTechniciansWithRate();
            GetAllTechniciansWithServiceId();
            GetAllTechniciansSearch();
            GetAllTechniciansWithServiceAreaId();
            UpdateTechnician();
            GetById();
            MyProfile();

        }
    }
}
