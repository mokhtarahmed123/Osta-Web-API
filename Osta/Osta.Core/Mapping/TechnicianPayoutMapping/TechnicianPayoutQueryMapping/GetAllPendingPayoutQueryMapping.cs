using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Domain.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianPayoutMapping
{
    public partial class TechnicianPayoutProfile
    {
        private void GetAllPending()
        {
            CreateMap<TechnicianPayout, GetAllPendingPayoutResult>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(src => src.RequestedAt))
                .ForMember(dest => dest.TechnicianId, opt => opt.MapFrom(src => src.TechnicianId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        }
    }
}
