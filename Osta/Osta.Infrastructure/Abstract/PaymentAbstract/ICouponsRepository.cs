using Osta.Domain.Entities.Customer;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.PaymentAbstract
{
    public interface ICouponsRepository : IGenericRepositoryAsync<Coupons>
    {
    }
}
