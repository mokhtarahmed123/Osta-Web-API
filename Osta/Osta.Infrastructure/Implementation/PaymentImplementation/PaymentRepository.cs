using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.PaymentImplementation
{
    public class PaymentRepository : GenericRepositoryAsync<Payment>, IPaymentRepository
    {
        #region Vars / Props
        private readonly DbSet<Payment> Payment;
        #endregion
        #region Constructor(s)
        public PaymentRepository(OstaContext dbContext) : base(dbContext)
        {
            Payment = dbContext.Set<Payment>();
        }

        public async Task<Payment> GetByBookingIdAsync(int BookingId)
        {
            return await Payment.FirstOrDefaultAsync(b => b.BookingId == BookingId);
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await Payment.FirstOrDefaultAsync(a => a.TransactionId == transactionId);


        }

        #endregion

    }
}
