using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration
{
    public class TechnicianPayoutConfiguration
        : IEntityTypeConfiguration<TechnicianPayout>
    {
        public void Configure(EntityTypeBuilder<TechnicianPayout> builder)
        {
            builder.ToTable("TechnicianPayout", "Technician");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.RequestedAt)
                .IsRequired();

            builder.Property(p => p.RejectionReason)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(p => p.CompletedAt)
                .IsRequired(false);


            builder.HasOne(p => p.Technician)
                .WithMany(t => t.TechnicianPayouts)
                .HasForeignKey(p => p.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.TechnicianId);
        }
    }
}