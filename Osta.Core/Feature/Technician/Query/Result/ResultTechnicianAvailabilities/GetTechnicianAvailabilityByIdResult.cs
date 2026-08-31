namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnicianAvailabilities
{
    public record GetTechnicianAvailabilityByIdResult
    {
        public int Id { get; set; }
        public string TechnicianId { get; set; }
        public string Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; }


    }
}
