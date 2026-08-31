namespace Osta.Core.Feature.ServiceArea.Query.Result
{
    public record GetServiceAreaByIdResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }

    }
}
