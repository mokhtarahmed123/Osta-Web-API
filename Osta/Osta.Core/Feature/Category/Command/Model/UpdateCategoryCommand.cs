using MediatR;
using Microsoft.AspNetCore.Http;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Category.Command.Model
{
    public record UpdateCategoryCommand(int Id) : IRequest<Response<string>>
    {
        public string Name { get; init; }
        public IFormFile? Image { get; init; } = null;
        public bool IsActive { get; init; } = true;

    }
}
