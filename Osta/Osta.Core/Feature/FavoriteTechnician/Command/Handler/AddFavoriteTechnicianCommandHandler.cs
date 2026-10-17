using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.FavoriteTechnician.Command.Model;
using Osta.Service.Abstract.CustomerAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.FavoriteTechnician.Command.Handler
{
    public class AddFavoriteTechnicianCommandHandler : ResponseHandler,
        IRequestHandler<
            AddFavoriteTechnicianCommand,
            Response<string>>
    {
        private readonly IFavoriteTechnicianService favoriteTechnicianService;
        private readonly ICurrentUserService currentUserService;

        public AddFavoriteTechnicianCommandHandler(IFavoriteTechnicianService favoriteTechnicianService, ICurrentUserService currentUserService)
        {
            this.favoriteTechnicianService = favoriteTechnicianService;
            this.currentUserService = currentUserService;
        }

        public async Task<Response<string>> Handle(
            AddFavoriteTechnicianCommand request,
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

            var favoriteTechnician =
                new Osta.Data.Entities.FavoriteTechnician
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    TechnicianId = request.TechnicianId

                };

            try
            {
                await favoriteTechnicianService.Add(
                    favoriteTechnician);

                return Success<string>(
                    "Technician added to favorites successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest<string>(
                    ex.Message);
            }
        }
    }
}
