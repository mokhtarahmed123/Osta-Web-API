using Osta.Data.Entities.Administration;
using Osta.Data.Enum;

namespace Osta.Service.Abstract.AdministrationAbstract
{
    public interface IComplaintService
    {
        Task Add(Complaint complaint, CancellationToken cancellationToken);
        Task Update(int Id, Complaint complaint, CancellationToken cancellationToken);
        Task Delete(int Id, CancellationToken cancellationToken);
        Task<IEnumerable<Complaint>> GetMyComplaints(string UserId, CancellationToken cancellationToken);
        Task<IEnumerable<Complaint>> GetAllComplaints(CancellationToken cancellationToken);
        Task<Complaint?> GetById(
          int id,
          CancellationToken cancellationToken);
        Task UpdateStatus(
            int id,
            ComplaintStatus status,
            CancellationToken cancellationToken);
        Task<IEnumerable<Complaint>> GetByBookingId(
    int bookingId,
    CancellationToken cancellationToken);

    }
}
