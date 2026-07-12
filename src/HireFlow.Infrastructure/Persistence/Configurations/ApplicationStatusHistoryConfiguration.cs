using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
{
	public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
	{
		// ---------- Table ----------
		builder.ToTable("application_status_histories");

		// ---------- Primary key ----------
		builder.HasKey(h => h.Id);

		builder.Property(h => h.Id)
			   .ValueGeneratedOnAdd();

		// ---------- Properties ----------
		builder.Property(h => h.OldStatus)
			   .HasConversion<string>()
			   .HasMaxLength(20)
			   .IsRequired();

		builder.Property(h => h.NewStatus)
			   .HasConversion<string>()
			   .HasMaxLength(20)
			   .IsRequired();

		builder.Property(h => h.ChangedAt)
			   .IsRequired();

		// ---------- Relationships ----------
		// JobApplication → StatusHistory is configured in JobApplicationConfiguration
		// Nothing extra needed here — EF Core picks it up automatically
	}
}
