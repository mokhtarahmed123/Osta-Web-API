using Osta.Core.Feature.Service.Query.Result;
using Osta.Core.Feature.ServiceArea.Query.Result;

namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnician
{
    public record GetTechnicianByIdResult
    {
        public string? Bio { get; set; }

        public bool IsVerified { get; set; }

        public double Rating { get; set; }

        public string? ProfilePicture { get; set; }

        public int TotalReviews { get; set; }

        public int CompletedBookings { get; set; }

        public int YearsOfExperience { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? ReasonOfReject { get; set; }

        public string Status { get; set; }

        public List<GetServiceByIdResult>? Services { get; set; }
        public List<GetAllServiceAreasResult>? Areas { get; set; }



    }
}
