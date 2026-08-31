using MediatR;
using Osta.Core.Bases;
using Osta.Core.Feature.Technician.Query.Result.ResultTechnician;
using Osta.Data.Enum;

namespace Osta.Core.Feature.Technician.Query.Model.TechnicianModel
{
    public record GetAllTechniciansSearchQuery : IRequest<Response<List<GetAllTechniciansSearchResult>>>
    {
        public bool? IsVerified { get; init; }

        public StatusOfTechnicianRequestEnum? Status { get; init; }


        public double? MinRating { get; init; }

        public int? MinYearsOfExperience { get; init; }

    }
}
