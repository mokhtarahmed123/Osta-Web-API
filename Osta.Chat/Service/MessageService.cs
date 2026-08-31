using Microsoft.EntityFrameworkCore;
using Osta.Chat.MessageService;
using Osta.Domain.Entities.Chat;
using Osta.Infrastructure.Abstract.IChatAbstract;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Chat.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMessageRepository messageRepository;

        public MessageService(IUnitOfWork unitOfWork, IMessageRepository messageRepository)
        {
            this.unitOfWork = unitOfWork;
            this.messageRepository = messageRepository;
        }
        public async Task<IEnumerable<Message>> GetMessageByBookingId(int BookingId, CancellationToken ct = default)
        {
            return await messageRepository.GetTableNoTracking(ct)
                   .Where(m => m.BookingId == BookingId)
                   .OrderBy(m => m.SentAt)
                   .ToListAsync();
        }

        public async Task SendMessage(Message message, CancellationToken ct = default)
        {
            await messageRepository.AddAsync(message, ct);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
