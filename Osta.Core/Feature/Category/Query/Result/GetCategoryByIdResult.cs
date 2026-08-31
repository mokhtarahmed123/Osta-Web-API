namespace Osta.Core.Feature.Category.Query.Result
{
    public record GetCategoryByIdResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
