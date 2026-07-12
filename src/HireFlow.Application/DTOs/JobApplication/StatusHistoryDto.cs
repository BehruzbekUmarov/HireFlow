namespace HireFlow.Application.DTOs.JobApplication;

public class StatusHistoryDto
{
	public string OldStatus { get; set; } = string.Empty;
	public string NewStatus { get; set; } = string.Empty;
	public DateTime ChangedAt { get; set; }
}
