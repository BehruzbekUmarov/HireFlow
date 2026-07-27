using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
	public void Configure(EntityTypeBuilder<Company> builder)
	{
		builder.ToTable("companies");

		builder.HasKey(c => c.Id);

		builder.Property(c => c.Name)
			   .HasMaxLength(200)
			   .IsRequired();

		builder.Property(c => c.Description)
			   .HasMaxLength(2000);  

		builder.Property(c => c.IsApproved)
			   .IsRequired()
			   .HasDefaultValue(false);

		builder.Property(c => c.CreatedAt)
			   .IsRequired();

		builder.HasOne(c => c.User)
			   .WithOne(u => u.Company)
			   .HasForeignKey<Company>(c => c.UserId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(c => c.UserId)
			   .IsUnique();

		builder.HasMany(c => c.Jobs)
			   .WithOne(j => j.Company)
			   .HasForeignKey(j => j.CompanyId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.HasQueryFilter(c => !c.IsDeleted);
	}
}