using HireFlow.Domain.Entities.Base;
using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class ApplicationStatusHistory : EntityBase
{
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
