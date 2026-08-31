using Microsoft.EntityFrameworkCore;
using Osta.Core.Feature.Payment.Query;
using Osta.Infrastructure.DataBase;

public class PaymentQueryService : IPaymentQueryService
{
    private readonly OstaContext _context;

    public PaymentQueryService(OstaContext context)
    {
        _context = context;
    }

    public async Task<List<GetMyPaymentsResult>> GetMyPaymentsAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await _context.Payments
            .Where(p => p.bookings.CustomerId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new GetMyPaymentsResult
            {
                Id = p.Id,
                BookingId = p.BookingId,
                Amount = p.Amount,
                Status = p.Status.ToString(),
                Method = p.Method.ToString(),
                TransactionId = p.TransactionId,
                CreatedAt = p.CreatedAt,
                TechnicianName = p.bookings.Technician.User.FullName
            })
            .ToListAsync(cancellationToken);
    }
}