namespace Osta.Core.Feature.FavoriteTechnician.Query.Result
{
    public record GetMyFavoriteResult
    {
        public string TechnicianId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
