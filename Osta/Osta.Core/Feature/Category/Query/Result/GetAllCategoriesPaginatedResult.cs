namespace Osta.Core.Feature.Category.Query.Result
{
    public record GetAllCategoriesPaginatedResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public bool IsActive { get; set; }
        public GetAllCategoriesPaginatedResult(int Id, string Name, string ImageUrl, bool IsActive)
        {
            this.Id = Id;
            this.Name = Name;
            this.ImageUrl = ImageUrl;
            this.IsActive = IsActive;
        }

    }
}
