namespace HireFlow.Application.DTOs.Admin;

public class UserSummaryDto
{
	public long Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
}
