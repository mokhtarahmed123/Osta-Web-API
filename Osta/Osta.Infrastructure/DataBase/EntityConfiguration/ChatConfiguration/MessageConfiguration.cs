using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Domain.Entities.Chat;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.ChatConfiguration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages", "Chat");


            builder.HasKey(m => m.Id);


            builder.Property(m => m.SenderId)
                .IsRequired();

            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(m => m.Content)
                .HasMaxLength(4000)
                .IsRequired();


            builder.Property(m => m.SentAt)
                .IsRequired();


            builder.Property(m => m.IsRead)
                .IsRequired()
                .HasDefaultValue(false);


            builder.Property(m => m.BookingId)
                .IsRequired();

            builder.HasOne(m => m.Booking)
                .WithMany()
                .HasForeignKey(m => m.BookingId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(m => m.BookingId);

            builder.HasIndex(m => m.SenderId);

            builder.HasIndex(m => new
            {
                m.BookingId,
                m.SentAt
            });
        }
    }
}