using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
{
	public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
	{
		builder.ToTable("application_status_histories");

		builder.HasKey(h => h.Id);

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
	}
}
