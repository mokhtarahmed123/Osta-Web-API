using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Osta.Data.Entities.Identity;
using Osta.Infrastructure.Abstract.AdministrationAbstract;
using Osta.Infrastructure.Abstract.AppointmentAbstract;
using Osta.Infrastructure.Abstract.BookingAbstract;
using Osta.Infrastructure.Abstract.CustomerAbstract;
using Osta.Infrastructure.Abstract.IChatAbstract;
using Osta.Infrastructure.Abstract.PaymentAbstract;
using Osta.Infrastructure.Abstract.ReviewAbstract;
using Osta.Infrastructure.Abstract.ServicesAbstract;
using Osta.Infrastructure.Abstract.TechnicianAbstract;
using Osta.Infrastructure.Caching;
using Osta.Infrastructure.Caching.Redis;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.Implementation.AdministrationImplementation;
using Osta.Infrastructure.Implementation.AppointmentImpelmention;
using Osta.Infrastructure.Implementation.BookingImplementation;
using Osta.Infrastructure.Implementation.BookingImplemention;
using Osta.Infrastructure.Implementation.ChatImplementation;
using Osta.Infrastructure.Implementation.CustomerImplementation;
using Osta.Infrastructure.Implementation.PaymentImplementation;
using Osta.Infrastructure.Implementation.ReviewImplementation;
using Osta.Infrastructure.Implementation.ServiceImplementation;
using Osta.Infrastructure.Implementation.ServiceImplemention;
using Osta.Infrastructure.Implementation.TechnicianImplementation;
using Osta.Infrastructure.Implementation.TechnicianImplemention;
using Osta.Infrastructure.InfrastructureBases;
using Osta.Infrastructure.Logging;
using Osta.SharedKernel.Logging;
using System.Text;

namespace Osta.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration Configuration)
        {

            #region Technician
            services.AddScoped<ITechnicianRepository, TechnicianRepository>();
            services.AddScoped<ITechnicianServiceRepository, TechnicianServiceRepository>();
            services.AddScoped<ITechnicianAvailabilityRepository, TechnicianAvailabilityRepository>();
            services.AddScoped<ITechnicianImagesRepository, TechnicianImagesRepository>();
            services.AddScoped<ITechnicianServiceAreasRepository, TechnicianServiceAreasRepository>();
            services.AddScoped<ITechnicianEarningRepository, TechnicianEarningRepository>();
            services.AddScoped<ITechnicianWalletRepository, TechnicianWalletRepository>();
            services.AddScoped<ITechnicianPayoutRepository, TechnicianPayoutRepository>();
            #endregion

            #region Service
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IServiceAreaRepository, ServiceAreaRepository>();
            #endregion
            #region Booking && Review && Appointment
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IBookingServicesRepository, BookingServicesRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();


            #endregion



            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IFavoriteTechnicianRepository, FavoriteTechnicianRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICouponsRepository, CouponsRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IUsageCouponsRepository, UsageCouponsRepository>();



            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddHttpClient();

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Osta Project v1",
                    Version = "v1",
                    Description = "Osta Web API Documentation"
                });
                options.SwaggerDoc("v2", new OpenApiInfo
                {

                    Title = "Osta Project V2",
                    Version = "v2",
                    Description = "Osta Web API Documentation"
                });

                options.EnableAnnotations();

                // JWT Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter only the JWT token. Swagger will add 'Bearer' automatically.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            #endregion

            #region Identity
            services.AddIdentity<User, Role>(
                opt =>
                {
                    opt.Password.RequireDigit = true;
                    opt.Password.RequireLowercase = true;
                    opt.Password.RequireUppercase = true;
                    opt.Password.RequiredLength = 8;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.User.RequireUniqueEmail = true;
                    opt.SignIn.RequireConfirmedEmail = true;

                    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    opt.Lockout.MaxFailedAccessAttempts = 5;
                    opt.Lockout.AllowedForNewUsers = true;
                    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                }

                )

                .AddEntityFrameworkStores<OstaContext>()
                .AddDefaultTokenProviders();
            #endregion
            #region Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                  .AddJwtBearer(options =>
                  {
                      var secretKey = Configuration["JWT:SecretKey"];
                      if (string.IsNullOrEmpty(secretKey))
                          throw new ArgumentNullException("JWT:SecretKey", "JWT SecretKey is missing in appsettings.json");

                      options.SaveToken = true;
                      options.RequireHttpsMetadata = false;
                      options.MapInboundClaims = false;

                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidIssuer = Configuration["JWT:IssuerIP"],

                          ValidateAudience = true,
                          ValidAudience = Configuration["JWT:AudienceIP"],

                          ValidateIssuerSigningKey = true,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                          RoleClaimType = "roleName",
                          ValidateLifetime = true,
                          ClockSkew = TimeSpan.Zero
                      };
                      options.Events = new JwtBearerEvents
                      {
                          OnMessageReceived = context =>
                          {
                              var accessToken = context.Request.Query["access_token"];
                              var path = context.HttpContext.Request.Path;
                              if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                                  context.Token = accessToken;
                              return Task.CompletedTask;
                          }
                      };

                  });

            #endregion




            return services;
        }
    }
}
