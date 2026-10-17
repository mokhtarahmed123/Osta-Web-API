using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Osta.Service.Abstract.AdministrationAbstract;
using Osta.Service.Abstract.AppointmentAbstract;
using Osta.Service.Abstract.CustomerAbstract;
using Osta.Service.Abstract.ReviewAbstract;
using Osta.Service.Abstract.ServicesAbstract;
using Osta.Service.Abstract.TechnicianAbstract;
using Osta.Service.Service;
using Osta.Service.Service.AdministrationServiceFolder;
using Osta.Service.Service.AppointmentServiceFolder;
using Osta.Service.Service.CustomerServiceFolder;
using Osta.Service.Service.ServicesServiceFolder;
using Osta.Service.Service.TechnicianServiceFolder;

namespace Osta.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            #region Service
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IServiceService, ServiceService>();
            #endregion

            #region Technician

            services.AddScoped<ITechnicianService, TechnicianService>();
            services.AddScoped<ITechnicianServiceService, TechnicianServiceService>();
            services.AddScoped<IServiceAreaService, ServiceAreaService>();
            services.AddScoped<ITechnicianServiceAreasService, TechnicianServiceAreasService>();
            services.AddScoped<ITechnicianAvailabilityService, TechnicianAvailabilityService>();
            services.AddScoped<ITechnicianImagesService, TechnicianImagesService>();
            services.AddScoped<ITechnicianEarningService, TechnicianEarningService>();
            services.AddScoped<ITechnicianWalletService, TechnicianWalletService>();
            services.AddScoped<ITechnicianPayoutService, TechnicianPayoutService>();
            #endregion

            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IBookingServiceService, BookingServiceService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddScoped<IFavoriteTechnicianService, FavoriteTechnicianService>();

            return services;
        }

    }
}
