using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.ServicesConfiguration
{
    public class ServiceConfiguration
        : IEntityTypeConfiguration<Data.Entities.Services.Service>
    {
        public void Configure(EntityTypeBuilder<Data.Entities.Services.Service> builder)
        {
            // Table
            builder.ToTable("Services", "Service");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            // Description
            builder.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Price
            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            // Image URL
            builder.Property(x => x.ImageUrl)
                .IsRequired(false);

            // Is Active
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Category Relationship
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Services)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service -> TechnicianServices
            builder.HasMany(x => x.TechnicianServices)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Service -> BookingServices
            builder.HasMany(x => x.BookingService)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}