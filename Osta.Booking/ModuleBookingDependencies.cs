using Microsoft.Extensions.DependencyInjection;
using Osta.Booking.Interface;
using Osta.Booking.Producer;
using Osta.Booking.Service;

namespace Osta.Booking
{
    public static class ModuleBookingDependencies
    {
        public static IServiceCollection AddBookingDependencies(this IServiceCollection services)
        {

            services.AddScoped<ISendBookingMessage, SendBookingMessage>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IMediaService, MediaService>();

            return services;


        }
    }
}