using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");

		builder.HasKey(u => u.Id);

		builder.Property(u => u.Email)
			   .HasMaxLength(256)
			   .IsRequired();

		builder.Property(u => u.PasswordHash)
			   .IsRequired();

		builder.Property(u => u.FullName)
			   .HasMaxLength(200)
			   .IsRequired();

		builder.Property(u => u.Role)
			   .HasConversion<string>()
			   .HasMaxLength(20)
			   .IsRequired();

		builder.Property(u => u.CreatedAt)
			   .IsRequired();

		builder.HasIndex(u => u.Email)
			   .IsUnique();

		builder.HasQueryFilter(u => !u.IsDeleted);
	}
}
