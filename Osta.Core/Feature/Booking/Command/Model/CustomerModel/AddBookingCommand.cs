using MediatR;
using Osta.Core.Bases;

namespace Osta.Core.Feature.Booking.Command.Model.CustomerModel
{
    public record AddBookingCommand : IRequest<Response<string>>
    {

        public string TechnicianId { get; set; } //  He Proposes The Date
        public int ServiceId { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Street { get; set; }
    }
}
