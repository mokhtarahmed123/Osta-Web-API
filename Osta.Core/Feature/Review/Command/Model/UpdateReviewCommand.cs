using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Review.Command.Model
{
    public record UpdateReviewCommand(int Id) : IRequest<Response<string>>
    {

        public int Rating { get; set; }
        public string? Comment { get; set; }

    }
}
