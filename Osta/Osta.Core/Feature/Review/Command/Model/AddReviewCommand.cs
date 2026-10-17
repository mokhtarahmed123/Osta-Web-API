using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Review.Command.Model
{
    public record AddReviewCommand : IRequest<Response<string>>
    {
        public int BookingId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
