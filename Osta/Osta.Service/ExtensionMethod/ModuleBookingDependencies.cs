using Microsoft.Extensions.DependencyInjection;
using Osta.Booking.Interface;
using Osta.Booking.Producer;
using Osta.Booking.Service;

namespace Osta.Service.ExtensionMethod;

public static class ModuleBookingDependencies
{
    public static IServiceCollection AddBookingDependencies(this IServiceCollection services)
    {


        #region Booking 


        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IMediaService, MediaService>();

        services.AddScoped<ISendBookingMessage, SendBookingMessage>();
        #endregion

        return services;


    }
}
