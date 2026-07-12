using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		// ---------- Table ----------
		builder.ToTable("refresh_tokens");

		// ---------- Primary key ----------
		builder.HasKey(t => t.Id);

		builder.Property(t => t.Id)
			   .ValueGeneratedOnAdd();

		// ---------- Properties ----------
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

		// ---------- Indexes ----------
		// Every incoming refresh request looks up by TokenHash
		// Without this index, it would scan the entire table each time
		builder.HasIndex(t => t.TokenHash);

		// ---------- Relationships ----------
		// User → RefreshTokens: one user, many tokens (one per device/session)
		builder.HasOne(t => t.User)
			   .WithMany(u => u.RefreshTokens)
			   .HasForeignKey(t => t.UserId)
			   .OnDelete(DeleteBehavior.Cascade);
		// Cascade: if a user is deleted, all their tokens are deleted too
	}
}
