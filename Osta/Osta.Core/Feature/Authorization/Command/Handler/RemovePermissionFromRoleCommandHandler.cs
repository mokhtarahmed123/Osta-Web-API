using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Authorization.Command.Model.PermissionModel;
using Osta.Identity.Authorization;

namespace Osta.Core.Feature.Authorization.Command.Handler
{
    public class RemovePermissionFromRoleCommandHandler : ResponseHandler, IRequestHandler<RemovePermissionFromRoleCommand, Response<string>>
    {

        private readonly IAuthorizationService authorizationService;

        public RemovePermissionFromRoleCommandHandler(IAuthorizationService authorizationService)
        {

            this.authorizationService = authorizationService;
        }
        public async Task<Response<string>> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await authorizationService.RemovePermissionFromRoleAsync(
                    request.PermissionId,
                    request.RoleId);

                return Success<string>(
                    "Permission removed from role successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound<string>(ex.Message);
            }
        }
    }
}
