using Osta.Domain.Entities.Chat;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Abstract.IChatAbstract
{
    public interface IMessageRepository : IGenericRepositoryAsync<Message>
    {
    }
}
