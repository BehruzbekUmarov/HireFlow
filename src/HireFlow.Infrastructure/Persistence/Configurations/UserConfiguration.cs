using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		// ---------- Table ----------
		builder.ToTable("users");

		// ---------- Primary key ----------
		builder.HasKey(u => u.Id);

		// EF Core will use IDENTITY (auto-increment) for long Id
		builder.Property(u => u.Id)
			   .ValueGeneratedOnAdd();

		// ---------- Properties ----------
		builder.Property(u => u.Email)
			   .HasMaxLength(256)
			   .IsRequired();

		builder.Property(u => u.PasswordHash)
			   .IsRequired();

		builder.Property(u => u.FullName)
			   .HasMaxLength(200)
			   .IsRequired();

		// Store enum as string ("Freelancer") not integer (0)
		// Makes the DB readable when you open it directly in pgAdmin
		builder.Property(u => u.Role)
			   .HasConversion<string>()
			   .HasMaxLength(20)
			   .IsRequired();

		builder.Property(u => u.CreatedAt)
			   .IsRequired();

		// ---------- Indexes ----------
		// Email must be unique — no two accounts with the same email
		builder.HasIndex(u => u.Email)
			   .IsUnique();

		// ---------- Relationships ----------
		// One User → One Company (configured from Company side)
		// One User → Many JobApplications (configured from JobApplication side)
		// One User → Many RefreshTokens (configured from RefreshToken side)
	}
}
