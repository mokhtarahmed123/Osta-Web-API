using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.CustomerConfiguration
{
    public class FavoriteTechnicianConfiguration : IEntityTypeConfiguration<FavoriteTechnician>
    {
        public void Configure(EntityTypeBuilder<FavoriteTechnician> builder)
        {
            builder.ToTable("FavoriteTechnicians", "Customer");

            builder.HasKey(x => new
            {
                x.CustomerId,
                x.TechnicianId
            });


            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.Technician)
                .WithMany(x => x.FavoriteTechnicians)
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}
