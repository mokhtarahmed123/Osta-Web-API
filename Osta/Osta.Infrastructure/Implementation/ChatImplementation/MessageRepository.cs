using Microsoft.EntityFrameworkCore;
using Osta.Domain.Entities.Chat;
using Osta.Infrastructure.Abstract.IChatAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.ChatImplementation
{
    public class MessageRepository : GenericRepositoryAsync<Message>, IMessageRepository
    {
        #region Vars / Props
        private readonly DbSet<Message> Message;
        #endregion
        #region Constructor(s)
        public MessageRepository(OstaContext dbContext) : base(dbContext)
        {
            Message = dbContext.Set<Message>();
        }

        #endregion

    }
}
