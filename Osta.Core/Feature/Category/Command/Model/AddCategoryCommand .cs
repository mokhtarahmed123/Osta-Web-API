using MediatR;
using Microsoft.AspNetCore.Http;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Category.Command.Model
{
    public record AddCategoryCommand : IRequest<Response<string>>
    {
        public string Name { get; init; } = string.Empty;
        public IFormFile? Image { get; init; }
        public bool IsActive { get; init; } = true;


    }
}
