using Osta.Core.Feature.Technician.Query.Result.ResultTechnicianPayout;
using Osta.Domain.Entities.Technician;

namespace Osta.Core.Mapping.TechnicianPayoutMapping
{
    public partial class TechnicianPayoutProfile
    {
        private void GetAll()
        {
            CreateMap<TechnicianPayout, GetAllMyPayoutsResult>();
        }
    }
}
