using MediatR;
using Microsoft.AspNetCore.Http;
using Osta.Core.Bases;
using Osta.Data.Enum;
using Osta.Domain.Enum;

namespace Osta.Core.Feature.MediaBooking.Command.Model
{
    public record AddMediaBookingCommand : IRequest<Response<string>>
    {
        public int BookingId { get; set; }
        public IFormFile File { get; set; }

        public MediaFileType FileType { get; set; }

        public RepairMediaTypeEnum RepairMediaType { get; set; }
        public string Description { get; set; }
    }
}
