using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
	public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
	{
		builder.ToTable("password_reset_tokens");
		builder.HasKey(t => t.Id);
		builder.Property(t => t.Id).ValueGeneratedOnAdd();
		builder.Property(t => t.TokenHash).HasMaxLength(500).IsRequired();
		builder.HasIndex(t => t.TokenHash);

		builder.HasOne(t => t.User)
			   .WithMany(u => u.PasswordResetTokens)
			   .HasForeignKey(t => t.UserId)
			   .OnDelete(DeleteBehavior.Cascade);
	}
}
