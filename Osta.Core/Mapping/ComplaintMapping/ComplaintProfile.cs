using AutoMapper;

namespace Osta.Core.Mapping.ComplaintMapping
{
    public partial class ComplaintProfile : Profile
    {
        public ComplaintProfile()
        {
            Add();
            GetByBookingId();
            GetAllComplaint();
            GetMyComplaintsAsUser();
            GetById();
        }

    }
}
