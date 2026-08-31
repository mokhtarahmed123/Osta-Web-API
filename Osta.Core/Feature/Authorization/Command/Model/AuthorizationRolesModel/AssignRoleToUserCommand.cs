using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Authorization.Command.Model.Roles;
public record AssignRoleToUserCommand(string UserId, string RoleId) : IRequest<Response<string>>;
