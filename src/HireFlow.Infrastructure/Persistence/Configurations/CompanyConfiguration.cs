using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
	public void Configure(EntityTypeBuilder<Company> builder)
	{
		// ---------- Table ----------
		builder.ToTable("companies");

		// ---------- Primary key ----------
		builder.HasKey(c => c.Id);

		builder.Property(c => c.Id)
			   .ValueGeneratedOnAdd();

		// ---------- Properties ----------
		builder.Property(c => c.Name)
			   .HasMaxLength(200)
			   .IsRequired();

		builder.Property(c => c.Description)
			   .HasMaxLength(2000);  // optional, no IsRequired()

		builder.Property(c => c.IsApproved)
			   .IsRequired()
			   .HasDefaultValue(false);

		builder.Property(c => c.CreatedAt)
			   .IsRequired();

		// ---------- Relationships ----------

		// One-to-One: Company belongs to one User
		// UserId is the foreign key
		// If the User is deleted, delete the Company too (Cascade)
		builder.HasOne(c => c.User)
			   .WithOne(u => u.Company)
			   .HasForeignKey<Company>(c => c.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		// Enforce the one-to-one at DB level:
		// No two companies can share the same UserId
		builder.HasIndex(c => c.UserId)
			   .IsUnique();

		// One-to-Many: Company has many Jobs
		// Configured here — Jobs will have company_id FK
		builder.HasMany(c => c.Jobs)
			   .WithOne(j => j.Company)
			   .HasForeignKey(j => j.CompanyId)
			   .OnDelete(DeleteBehavior.Cascade);
		// Cascade: if a company is deleted, all its jobs are deleted too
	}
}