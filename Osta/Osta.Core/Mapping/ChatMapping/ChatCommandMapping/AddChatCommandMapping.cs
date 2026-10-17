using Osta.Core.Feature.Chat.Command.Model;
using Osta.Domain.Entities.Chat;

namespace Osta.Core.Mapping.ChatMapping
{
    public partial class ChatProfile
    {
        private void Add()
        {
            CreateMap<SendMessageCommand, Message>()
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId));
        }


    }
}
