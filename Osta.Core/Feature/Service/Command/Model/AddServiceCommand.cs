using MediatR;
using Microsoft.AspNetCore.Http;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Service.Command.Model
{
    public record AddServiceCommand : IRequest<Response<string>>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsActive { get; set; } = false;


    }
}
