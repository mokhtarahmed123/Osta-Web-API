using Osta.Core.Feature.Complaint.Command.Model;
using Osta.Data.Entities.Administration;
using Osta.Data.Enum;

namespace Osta.Core.Mapping.ComplaintMapping
{
    public partial class ComplaintProfile
    {
        private void Add()
        {
            CreateMap<AddComplaintCommand, Complaint>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ComplaintStatus.Open))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}
