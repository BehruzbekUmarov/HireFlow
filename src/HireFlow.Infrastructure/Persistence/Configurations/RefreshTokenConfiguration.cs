using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("refresh_tokens");

		builder.HasKey(t => t.Id);

		builder.Property(t => t.TokenHash)
			   .HasMaxLength(500)
			   .IsRequired();

		builder.Property(t => t.ExpiresAt)
			   .IsRequired();

		builder.Property(t => t.Revoked)
			   .IsRequired()
			   .HasDefaultValue(false);

		builder.Property(t => t.CreatedAt)
			   .IsRequired();

		builder.HasIndex(t => t.TokenHash);

		builder.HasOne(t => t.User)
			   .WithMany(u => u.RefreshTokens)
			   .HasForeignKey(t => t.UserId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}
