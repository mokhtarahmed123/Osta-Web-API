using MediatR;
using Microsoft.AspNetCore.Http;
using Osta.Core.Bases;
using Osta.Data.Enum;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.MediaBooking.Command.Model
{
    public record UpdateMediaBookingCommand(int Id) : IRequest<Response<string>>
    {
        public IFormFile File { get; set; }

        public MediaFileType FileType { get; set; }

        public RepairMediaTypeEnum RepairMediaType { get; set; }
        public string Description { get; set; }

    }
}
