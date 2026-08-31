
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Data.Entities.Administration;
using Osta.Data.Entities.Booking;
using Osta.Data.Entities.Identity;
using Osta.Data.Entities.Services;
using Osta.Data.Entities.Technician;
using Osta.Domain.Entities.Appointment;
using Osta.Domain.Entities.Chat;
using Osta.Domain.Entities.Customer;
using Osta.Domain.Entities.Identity;
using Osta.Domain.Entities.Payment___Reviews;
using Osta.Domain.Entities.Technician;
using Osta.Infrastructure.DataBase.EntityConfiguration;
using Osta.Infrastructure.Seed;

namespace Osta.Infrastructure.DataBase
{
    public class OstaContext : IdentityDbContext<User, Role, string>
    {
        #region Tables
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TechnicianImages> TechnicianImages { get; set; }

        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<FavoriteTechnician> FavoriteTechnicians { get; set; }
        public DbSet<Technicians> Technicians { get; set; }
        public DbSet<TechnicianService> TechnicianServices { get; set; }
        public DbSet<TechnicianServiceArea> TechnicianServiceAreas { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<TechnicianAvailability> TechnicianAvailabilities { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }



        public DbSet<Media> Media { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Data.Entities.Services.Service> Services { get; set; }
        public DbSet<ServiceArea> ServiceAreas { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Coupons> Coupons { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        public DbSet<TechnicianEarning> TechnicianEarning { get; set; }
        public DbSet<TechnicianWallet> TechnicianWallet { get; set; }
        public DbSet<TechnicianPayout> TechnicianPayouts { get; set; }
        #endregion 
        public OstaContext()
        {

        }
        public OstaContext(DbContextOptions<OstaContext> options)
            : base(options)
        {


        }



        protected override void OnModelCreating(ModelBuilder builder)

        {


            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("AspNetUsers", "Identity");
            builder.Entity<Role>().ToTable("AspNetRoles", "Identity");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "Identity");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "Identity");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "Identity");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "Identity");

            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "Identity");


            builder.Entity<FavoriteTechnician>()
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FavoriteTechnician>()
                .HasOne(x => x.Technician)
               .WithMany(t => t.FavoriteTechnicians)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Bookings>()
            .HasOne(b => b.Appointment)
            .WithOne(a => a.Booking)
        .HasForeignKey<Appointment>(a => a.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.ApplyConfiguration(new CouponUsageConfiguration());
            builder.ApplyConfiguration(new TechnicianEarningConfiguration());
            builder.ApplyConfiguration(new TechnicianWalletConfiguration());
            builder.ApplyConfiguration(new TechnicianPayoutConfiguration());

            builder.Entity<Permission>()
                .HasData(PermissionSeeder.GetPermissions());

            builder.Entity<Technicians>()
           .Property(x => x.Status)
        .HasConversion<string>();

            builder.Entity<FavoriteTechnician>()
           .HasKey(f => new { f.CustomerId, f.TechnicianId });

            builder.Entity<RolePermission>()
           .HasKey(f => new { f.RoleId, f.PermissionId });

            builder.Entity<TechnicianService>()
           .HasKey(f => new { f.ServiceId, f.TechnicianId });

            builder.Entity<TechnicianServiceArea>()
           .HasKey(f => new { f.ServiceAreaId, f.TechnicianId });


            builder.Entity<BookingService>()
       .HasKey(bs => new { bs.BookingId, bs.ServiceId });


            builder.Entity<TechnicianAvailability>()
            .HasIndex(t => new { t.TechnicianId, t.DayOfWeek })
             .IsUnique();


            builder.Entity<Review>()
        .HasIndex(r => r.BookingId)
        .IsUnique();
        }
    }
}

