namespace Osta.Core.Feature.Service.Query.Result
{
    public class GetAllServiceResult
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

    }
}
