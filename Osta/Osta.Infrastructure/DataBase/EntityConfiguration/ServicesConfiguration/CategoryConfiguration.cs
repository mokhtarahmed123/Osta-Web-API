using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Services;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.ServicesConfiguration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category", "Service");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Name
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Image URL
            builder.Property(x => x.ImageUrl)
                .IsRequired(false);

            // Is Active
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Category -> Services
            builder.HasMany(x => x.Services)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
