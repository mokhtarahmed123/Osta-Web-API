using AutoMapper;

namespace Osta.Core.Mapping.ServiceMapping
{
    public partial class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            AddServiceCommandMapping();
            GetAllServiceQueryMapping();
            GetById();
            UpdateService();
        }

    }
}
