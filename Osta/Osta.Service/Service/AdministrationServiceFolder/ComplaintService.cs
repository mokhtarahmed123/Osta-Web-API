using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Administration;
using Osta.Data.Enum;
using Osta.Infrastructure.Abstract.AdministrationAbstract;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.SharedKernel.Logging;

namespace Osta.Service.Service.AdministrationServiceFolder
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository complaintRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILoggerService loggerService;

        public ComplaintService(
            IComplaintRepository complaintRepository,
            IUnitOfWork unitOfWork,
            ILoggerService loggerService)
        {
            this.complaintRepository = complaintRepository;
            this.unitOfWork = unitOfWork;
            this.loggerService = loggerService;
        }

        public async Task Add(
            Complaint complaint,
            CancellationToken cancellationToken)
        {
            if (complaint is null)
                throw new ArgumentNullException(nameof(complaint));

            await complaintRepository.AddAsync(complaint, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            loggerService.LogInformation(
                $"Complaint added successfully for Booking Id: {complaint.BookingId}");
        }

        public async Task Update(
            int id,
            Complaint complaint,
            CancellationToken cancellationToken)
        {
            if (complaint is null)
                throw new ArgumentNullException(nameof(complaint));

            var existingComplaint =
                await complaintRepository.GetByIdAsync(id, cancellationToken);

            if (existingComplaint is null)
                throw new KeyNotFoundException(
                    "Complaint not found.");

            // Only Description can be updated
            existingComplaint.Description =
                complaint.Description;

            await complaintRepository.UpdateAsync(
                existingComplaint, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            loggerService.LogInformation(
                $"Complaint {id} updated successfully.");
        }

        public async Task Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var complaint =
                await complaintRepository.GetByIdAsync(id, cancellationToken);

            if (complaint is null)
                throw new KeyNotFoundException(
                    "Complaint not found.");

            await complaintRepository.DeleteAsync(
                complaint, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            loggerService.LogInformation(
                $"Complaint {id} deleted successfully.");
        }

        public async Task<Complaint?> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            return await complaintRepository
                .GetTableAsTracking(cancellationToken)
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<IEnumerable<Complaint>>
            GetMyComplaints(
                string userId,
                CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException(
                    "User Id is required.",
                    nameof(userId));

            return await complaintRepository
                .GetTableAsTracking(cancellationToken)
                .Include(x => x.Booking)
                .Where(x =>
                    x.Booking.CustomerId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Complaint>>
            GetAllComplaints(
                CancellationToken cancellationToken)
        {
            return await complaintRepository
                .GetTableAsTracking(cancellationToken)
                .Include(x => x.Booking)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Complaint>>
            GetByBookingId(
                int bookingId,
                CancellationToken cancellationToken)
        {
            return await complaintRepository
                .GetTableAsTracking(cancellationToken)
                .Include(x => x.Booking)
                .Where(x => x.BookingId == bookingId)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateStatus(
            int id,
            ComplaintStatus status,
            CancellationToken cancellationToken)
        {
            var complaint =
                await complaintRepository.GetByIdAsync(id, cancellationToken);

            if (complaint is null)
                throw new KeyNotFoundException(
                    "Complaint not found.");

            complaint.Status = status;

            await complaintRepository.UpdateAsync(
                complaint, cancellationToken);

            await unitOfWork.SaveChangesAsync();

            loggerService.LogInformation(
                $"Complaint {id} status changed to {status}.");
        }
    }
}