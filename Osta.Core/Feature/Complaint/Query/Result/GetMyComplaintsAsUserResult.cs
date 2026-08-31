namespace Osta.Core.Feature.Complaint.Query.Result
{
    public record GetMyComplaintsAsUserResult
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime BookingDate { get; set; }
        public string CustomerId { get; set; }
        public string TechnicianId { get; set; }

    }
}
