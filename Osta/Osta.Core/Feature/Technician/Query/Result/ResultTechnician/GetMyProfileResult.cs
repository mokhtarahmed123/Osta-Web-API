namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnician
{
    public record GetMyProfileResult
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; } = null;
        public string? Bio { get; set; } = null;
        public bool IsVerified { get; set; }
        public int Rating { get; set; }
        public int YearsOfExperience { get; set; }
        public int CompletedBookings { get; set; }
        public string ReasonOfReject { get; set; }
        public string Status { get; set; }
        public string NationalId { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
