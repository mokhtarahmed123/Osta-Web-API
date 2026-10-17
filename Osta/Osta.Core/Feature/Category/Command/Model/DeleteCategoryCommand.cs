using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Category.Command.Model
{
    public record DeleteCategoryCommand(int Id) : IRequest<Response<string>>;

}
