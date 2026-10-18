using Microsoft.Extensions.DependencyInjection;
using Osta.Core.Abstract.AdministrationAbstract;
using Osta.Core.Abstract.AppointmentAbstract;
using Osta.Core.Abstract.CustomerAbstract;
using Osta.Core.Abstract.ReviewAbstract;
using Osta.Core.Abstract.ServicesAbstract;
using Osta.Core.Abstract.TechnicianAbstract;
using Osta.Service.Service;
using Osta.Service.Service.AdministrationServiceFolder;
using Osta.Service.Service.AppointmentServiceFolder;
using Osta.Service.Service.CustomerServiceFolder;
using Osta.Service.Service.ServicesServiceFolder;
using Osta.Service.Service.TechnicianServiceFolder;

namespace Osta.Service.ExtensionMethod
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


            #region Appoinment
            services.AddScoped<IAppointmentService, AppointmentService>();

            #endregion








            services.AddScoped<IBookingServiceService, BookingServiceService>();

            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddScoped<IFavoriteTechnicianService, FavoriteTechnicianService>();




            return services;
        }

    }
}
