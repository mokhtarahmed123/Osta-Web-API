using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.TechnicianConfiguration
{
    public class TechnicianImagesConfiguration
        : IEntityTypeConfiguration<TechnicianImages>
    {
        public void Configure(EntityTypeBuilder<TechnicianImages> builder)
        {
            // Table
            builder.ToTable("TechnicianImages", "Technician");

            // Primary Key
            builder.HasKey(x => x.TechnicianId);

            // Technician Id
            builder.Property(x => x.TechnicianId)
                .IsRequired();

            // Technician Relationship - One To One
            builder.HasOne(x => x.Technician)
                .WithOne()
                .HasForeignKey<TechnicianImages>(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);

            // Profile Picture - Optional
            builder.Property(x => x.ProfilePicture)
                .IsRequired(false);

            // Front National ID
            builder.Property(x => x.FrontNationalIdImage)
                .IsRequired();

            // Back National ID
            builder.Property(x => x.BackNationalIdImage)
                .IsRequired();
        }
    }
}