using AutoMapper;

namespace Osta.Core.Mapping.ChatMapping
{
    public partial class ChatProfile : Profile
    {
        public ChatProfile()
        {
            Add();
            SendNotify();
            GetAll();
        }

    }
}
