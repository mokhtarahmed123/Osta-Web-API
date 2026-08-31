using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Model.TechnicianModel;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Osta.Data.Entities.Identity;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.SharedKernel.Identity;

namespace Osta.Core.Feature.Technician.Query.Handler.TechnicianQueryHandler
{
    public class GetMyProfileQueryHandler
        : ResponseHandler,
          IRequestHandler<GetMyProfileQuery, Response<GetMyProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly ITechnicianService _technicianService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public GetMyProfileQueryHandler(
            IMapper mapper,
            ITechnicianService technicianService,
            ICurrentUserService currentUserService,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _technicianService = technicianService;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Response<GetMyProfileResult>> Handle(
            GetMyProfileQuery request,
            CancellationToken cancellationToken)
        {
            var technicianId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(technicianId))
                return Unauthorized<GetMyProfileResult>(
                    "User is not authenticated.");

            var technician = await _technicianService.MyProfile(
                technicianId,
                cancellationToken);

            if (technician is null)
                return NotFound<GetMyProfileResult>(
                    "Technician profile not found.");

            var result = _mapper.Map<GetMyProfileResult>(technician);

            return Success(result);
        }
    }
}