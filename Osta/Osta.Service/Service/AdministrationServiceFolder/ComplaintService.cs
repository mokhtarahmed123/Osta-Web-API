using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities.Administration;
<<<<<<< HEAD
=======
using Osta.Data.Entities.Booking;
>>>>>>> c73a0b6 (Initial commit)
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

<<<<<<< HEAD
=======

>>>>>>> c73a0b6 (Initial commit)
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
<<<<<<< HEAD
                throw new KeyNotFoundException(
                    "Complaint not found.");

            // Only Description can be updated
=======
                throw new KeyNotFoundException("Complaint not found.");


>>>>>>> c73a0b6 (Initial commit)
            existingComplaint.Description =
                complaint.Description;

            await complaintRepository.UpdateAsync(
                existingComplaint, cancellationToken);

            await unitOfWork.SaveChangesAsync();

<<<<<<< HEAD
            loggerService.LogInformation(
                $"Complaint {id} updated successfully.");
=======
            loggerService.LogInformation($"Complaint {id} updated successfully.");
>>>>>>> c73a0b6 (Initial commit)
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
<<<<<<< HEAD
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

=======
                .Where(x => x.Id == id)
                .Select(x => new Complaint
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Description = x.Description,
                    Status = x.Status,
                    BookingId = x.Booking.Id,
                    Booking = new Bookings
                    {

                        BookingDate = x.Booking.BookingDate,
                        CustomerId = x.Booking.CustomerId,
                        TechnicianId = x.Booking.TechnicianId
                    }
                })
                .FirstOrDefaultAsync(cancellationToken);
        }


>>>>>>> c73a0b6 (Initial commit)
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
<<<<<<< HEAD
                .Include(x => x.Booking)
=======
                  .Select(x => new Complaint
                  {
                      Id = x.Id,
                      CreatedAt = x.CreatedAt,
                      Description = x.Description,
                      Status = x.Status,
                      BookingId = x.Booking.Id,
                      Booking = new Bookings
                      {

                          BookingDate = x.Booking.BookingDate,
                          CustomerId = x.Booking.CustomerId,
                          TechnicianId = x.Booking.TechnicianId
                      }
                  })
>>>>>>> c73a0b6 (Initial commit)
                .Where(x =>
                    x.Booking.CustomerId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Complaint>>
            GetAllComplaints(
                CancellationToken cancellationToken)
        {
            return await complaintRepository
<<<<<<< HEAD
                .GetTableAsTracking(cancellationToken)
                .Include(x => x.Booking)
                .ToListAsync(cancellationToken);
        }

=======
         .GetTableAsTracking(cancellationToken)
                  .Select(x => new Complaint
                  {
                      Id = x.Id,
                      CreatedAt = x.CreatedAt,
                      Description = x.Description,
                      Status = x.Status,
                      BookingId = x.Booking.Id,
                      Booking = new Bookings
                      {

                          BookingDate = x.Booking.BookingDate,
                          CustomerId = x.Booking.CustomerId,
                          TechnicianId = x.Booking.TechnicianId
                      }
                  })

         .ToListAsync(cancellationToken);
        }



>>>>>>> c73a0b6 (Initial commit)
        public async Task<IEnumerable<Complaint>>
            GetByBookingId(
                int bookingId,
                CancellationToken cancellationToken)
        {
            return await complaintRepository
                .GetTableAsTracking(cancellationToken)
                .Include(x => x.Booking)
                .Where(x => x.BookingId == bookingId)
<<<<<<< HEAD
=======
             .Select(x => new Complaint
             {
                 Id = x.Id,
                 CreatedAt = x.CreatedAt,
                 Description = x.Description,
                 Status = x.Status,
                 Booking = new Bookings
                 {
                     BookingDate = x.Booking.BookingDate,
                     CustomerId = x.Booking.CustomerId,
                     TechnicianId = x.Booking.TechnicianId
                 }
             })
>>>>>>> c73a0b6 (Initial commit)
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