namespace HireFlow.Domain.Entities.Base;

public abstract class AuditableEntity : EntityBase
{
	public DateTime CreatedAt { get; set; }
	public required string CreatedBy { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public string? UpdatedBy { get; set; }
}
