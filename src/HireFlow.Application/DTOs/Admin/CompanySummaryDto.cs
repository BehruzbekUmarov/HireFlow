namespace HireFlow.Application.DTOs.Admin;

public class CompanySummaryDto
{
	public long Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string OwnerEmail { get; set; } = string.Empty;
	public bool IsApproved { get; set; }
	public int JobCount { get; set; }
	public DateTime CreatedAt { get; set; }
}
