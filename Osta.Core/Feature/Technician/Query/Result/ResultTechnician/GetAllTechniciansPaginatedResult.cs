namespace Osta.Core.Feature.Technician.Query.Result.ResultTechnician
{
    public record GetAllTechniciansPaginatedResult(string Id,
    string? Bio,
    bool IsVerified,
    double Rating,
    int TotalReviews,
    int CompletedBookings,
    int YearsOfExperience,
    DateTime CreatedAt,
    string? ReasonOfReject,
    string Status)
  ;
}
