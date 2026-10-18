using Microsoft.EntityFrameworkCore;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.PaymentImplementation
{
    public class PaymentRepository : GenericRepositoryAsync<Osta.Data.Entities.Payment>, IPaymentRepository
    {
        #region Vars / Props
        private readonly DbSet<Osta.Data.Entities.Payment> Payment;
        #endregion
        #region Constructor(s)
        public PaymentRepository(OstaContext dbContext) : base(dbContext)
        {
            Payment = dbContext.Set<Osta.Data.Entities.Payment>();
        }

        public async Task<Osta.Data.Entities.Payment> GetByBookingIdAsync(int BookingId)
        {
            return await Payment.FirstOrDefaultAsync(b => b.BookingId == BookingId);
        }

        public async Task<Osta.Data.Entities.Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await Payment.FirstOrDefaultAsync(a => a.TransactionId == transactionId);

        }

        #endregion

    }
}
