using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.TechnicianConfiguration
{
    public class TechnicianServiceConfiguration
        : IEntityTypeConfiguration<TechnicianService>
    {
        public void Configure(EntityTypeBuilder<TechnicianService> builder)
        {

            builder.ToTable("TechnicianServices", "Technician");

            builder.HasKey(x => new
            {
                x.TechnicianId,
                x.ServiceId
            });


            builder.HasOne(x => x.Technician)
                .WithMany(x => x.TechnicianServices)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.Service)
                .WithMany(x => x.TechnicianServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}