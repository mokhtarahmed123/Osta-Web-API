using AutoMapper;

namespace Osta.Core.Mapping.ServiceAreaMapping
{
    public partial class ServiceAreaProfile : Profile
    {
        public ServiceAreaProfile()
        {
            Add();
            Update();
            GetById();
            GetAll();
        }
    }
}
