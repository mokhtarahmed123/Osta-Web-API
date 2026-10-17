using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Administration;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.Administration
{
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.ToTable("Complaints", "Administration");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Description)
                .HasMaxLength(2000)
                .IsRequired();


            builder.Property(c => c.Status)
                .IsRequired();


            builder.Property(c => c.CreatedAt)
                .IsRequired();


            builder.HasOne(c => c.Booking)
                .WithMany(b => b.Complaints)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
