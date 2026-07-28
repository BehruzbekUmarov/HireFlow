namespace HireFlow.Application.Events;

public class ApplicationStatusChangedEvent
{
	public string FreelancerEmail { get; set; } = string.Empty;
	public string FreelancerFullName { get; set; } = string.Empty;
	public string JobTitle { get; set; } = string.Empty;
	public string CompanyName { get; set; } = string.Empty;
	public string OldStatus { get; set; } = string.Empty;
	public string NewStatus { get; set; } = string.Empty;
	public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}