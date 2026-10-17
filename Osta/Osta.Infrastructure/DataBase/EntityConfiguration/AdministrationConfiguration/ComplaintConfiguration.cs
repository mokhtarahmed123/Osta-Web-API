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
<<<<<<< HEAD
            builder.HasKey(c => c.Id);
=======

            builder.HasKey(c => c.Id);

>>>>>>> c73a0b6 (Initial commit)
            builder.Property(c => c.Description)
                .HasMaxLength(2000)
                .IsRequired();


            builder.Property(c => c.Status)
                .IsRequired();


            builder.Property(c => c.CreatedAt)
                .IsRequired();


<<<<<<< HEAD

=======
>>>>>>> c73a0b6 (Initial commit)
            builder.HasOne(c => c.Booking)
                .WithMany(b => b.Complaints)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
