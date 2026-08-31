using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.ServiceArea.Command.Model
{
    public record UpdateServiceAreaCommand(int Id) : IRequest<Response<string>>
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }

    }
}
