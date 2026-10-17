using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Technician;

namespace Osta.Infrastructure.DataBase.EntityConfiguration
{
    public class TechnicianWalletConfiguration
        : IEntityTypeConfiguration<TechnicianWallet>
    {
        public void Configure(
            EntityTypeBuilder<TechnicianWallet> builder)
        {
            builder.ToTable("TechnicianWallet", "Technician");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(w => w.UpdatedAt)
                .IsRequired();

            builder.HasOne(w => w.Technician)
                .WithOne(t => t.TechnicianWallet)
                .HasForeignKey<TechnicianWallet>(
                    w => w.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(w => w.TechnicianId)
                .IsUnique();
        }
    }
}