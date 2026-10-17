using AutoMapper;

namespace Osta.Core.Mapping.TechnicianPayoutMapping
{
    public partial class TechnicianPayoutProfile : Profile
    {
        public TechnicianPayoutProfile()
        {
            GetAll();
            GetById();
            GetAllPending();
        }
    }
}
