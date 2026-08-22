using HireFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HireFlow.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
	public void Configure(EntityTypeBuilder<Message> builder)
	{
		builder.ToTable("messages");
		builder.HasKey(m => m.Id);
		builder.Property(m => m.Id).ValueGeneratedOnAdd();

		builder.Property(m => m.Content)
			   .HasMaxLength(2000)
			   .IsRequired();

		builder.Property(m => m.IsRead).HasDefaultValue(false);
		builder.Property(m => m.SentAt).IsRequired();

		builder.HasOne(m => m.Application)
			   .WithMany(a => a.Messages)
			   .HasForeignKey(m => m.ApplicationId)
			   .OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(m => m.Sender)
			   .WithMany(u => u.SentMessages)
			   .HasForeignKey(m => m.SenderId)
			   .OnDelete(DeleteBehavior.NoAction);

		builder.HasIndex(m => m.ApplicationId);
		builder.HasIndex(m => new { m.ApplicationId, m.SentAt });
	}
}
