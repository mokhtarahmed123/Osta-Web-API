using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Osta.Data.Entities.Identity;

namespace Osta.Infrastructure.DataBase.EntityConfiguration.IdentityConfiguration
{
    public class RefreshTokenConfiguration
        : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {

            builder.ToTable("RefreshTokens", "Identity");


            builder.HasKey(x => x.Id);


            builder.Property(x => x.refreshToken)
                .IsRequired()
                .HasMaxLength(200);


            builder.Property(x => x.Expires)
                .IsRequired();


            builder.Property(x => x.Created)
                .IsRequired();


            builder.Property(x => x.Revoked)
                .IsRequired(false);


            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}