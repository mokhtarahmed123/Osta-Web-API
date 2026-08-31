using Osta.Data.Enum;

namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnician
{
    public record GetAllTechniciansResult

    {
        public string Id { get; set; }
        public string? Bio { get; set; }

        public bool IsVerified { get; set; }

        public double Rating { get; set; }

        public string ProfilePicture { get; set; }

        public int TotalReviews { get; set; }

        public int CompletedBookings { get; set; }

        public int YearsOfExperience { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? ReasonOfReject { get; set; }

        public StatusOfTechnicianRequestEnum Status { get; set; }

        public ICollection<Data.Entities.Technician.TechnicianServiceArea> TechnicianServiceArea { get; set; }
        public ICollection<Data.Entities.Technician.TechnicianService> TechnicianService { get; set; }

    }
}
