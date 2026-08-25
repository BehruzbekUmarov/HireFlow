namespace HireFlow.Application.Events;

public class PasswordResetRequestedEvent
{
	public string ToEmail { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string RawToken { get; set; } = string.Empty;

	public DateTime ExpiresAt { get; set; }
}