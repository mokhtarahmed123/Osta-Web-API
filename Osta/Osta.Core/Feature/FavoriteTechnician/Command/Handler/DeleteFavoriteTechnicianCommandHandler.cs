using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.FavoriteTechnician.Command.Model;
using Osta.Service.Abstract.CustomerAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.FavoriteTechnician.Command.Handler
{
    public class DeleteFavoriteTechnicianCommandHandler : ResponseHandler, IRequestHandler<
            DeleteFavoriteTechnicianCommand,
            Response<string>>
    {
        private readonly IFavoriteTechnicianService favoriteTechnicianService;
        private readonly ICurrentUserService currentUserService;

        public DeleteFavoriteTechnicianCommandHandler(IFavoriteTechnicianService favoriteTechnicianService, ICurrentUserService currentUserService)
        {
            this.favoriteTechnicianService = favoriteTechnicianService;
            this.currentUserService = currentUserService;
        }
        public async Task<Response<string>> Handle(
    DeleteFavoriteTechnicianCommand request,
    CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.TechnicianId))
                return BadRequest<string>(
                    "Technician Id is required.");

            var customerId =
                currentUserService.UserId;

            if (string.IsNullOrEmpty(customerId))
                throw new UnauthorizedAccessException(
                    "You are not authorized.");

            try
            {
                await favoriteTechnicianService.Delete(
                    customerId,
                    request.TechnicianId);

                return Success<string>(
                    "Technician removed from favorites successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound<string>(
                    ex.Message);
            }
        }

    }
}
