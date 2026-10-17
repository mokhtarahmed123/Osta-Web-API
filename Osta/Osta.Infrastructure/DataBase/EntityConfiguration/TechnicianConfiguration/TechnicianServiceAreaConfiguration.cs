using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.TechnicianConfiguration
{
    public class TechnicianServiceAreaConfiguration
        : IEntityTypeConfiguration<TechnicianServiceArea>
    {
        public void Configure(EntityTypeBuilder<TechnicianServiceArea> builder)
        {
            // Table
            builder.ToTable("TechnicianServiceAreas", "Technician");

            // Composite Primary Key
            builder.HasKey(x => new
            {
                x.TechnicianId,
                x.ServiceAreaId
            });

            // Technician Relationship
            builder.HasOne(x => x.Technician)
                .WithMany(x => x.TechnicianServiceArea)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);

            // Service Area Relationship
            builder.HasOne(x => x.ServiceArea)
                .WithMany(x => x.TechnicianServiceAreas)
                .HasForeignKey(x => x.ServiceAreaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}