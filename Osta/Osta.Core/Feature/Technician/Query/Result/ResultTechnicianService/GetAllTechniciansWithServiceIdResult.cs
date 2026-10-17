namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnicianService
{
    public record GetAllTechniciansWithServiceIdResult
    {
        public string Id { get; set; }
        public string? Bio { get; set; }

        public bool IsVerified { get; set; }

        public double Rating { get; set; }

        public string? ProfilePicture { get; set; }

        public int TotalReviews { get; set; }

        public int CompletedBookings { get; set; }

        public int YearsOfExperience { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }
        public string? ReasonOfReject { get; set; }


        public string ServiceName { get; set; }
        public double Price { get; set; }



    }
}
