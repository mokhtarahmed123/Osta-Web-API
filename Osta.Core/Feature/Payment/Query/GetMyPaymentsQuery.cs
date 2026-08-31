using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Payment.Query
{
    public record GetMyPaymentsQuery() : IRequest<Response<List<GetMyPaymentsResult>>>;
    public record GetMyPaymentsResult
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? TechnicianName { get; set; }

    }
}
