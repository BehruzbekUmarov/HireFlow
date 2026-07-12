using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class ApplicationStatusHistory
{
	public long Id { get; set; } 
	public long ApplicationId { get; set; }
	public ApplicationStatus OldStatus { get; set; }
	public ApplicationStatus NewStatus { get; set; }
	public DateTime ChangedAt { get; set; }

	public JobApplication? JobApplication { get; set; }
	public ApplicationStatusHistory()
	{
		ChangedAt = DateTime.UtcNow;
	}
}
