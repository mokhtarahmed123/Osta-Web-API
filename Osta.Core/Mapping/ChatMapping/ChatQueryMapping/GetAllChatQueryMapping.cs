using Osta.Chat.Model;
using Osta.Domain.Entities.Chat;

namespace Osta.Core.Mapping.ChatMapping
{
    public partial class ChatProfile
    {
        private void GetAll()
        {
            CreateMap<Message, MessageModel>();
        }
    }
}
