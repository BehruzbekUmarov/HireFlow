namespace HireFlow.Application.Events;

public class PasswordResetRequestedEvent
{
	public string ToEmail { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;

	// The raw token goes in the email link — e.g:
	// https://hireflow.com/reset-password?token=abc123
	public string RawToken { get; set; } = string.Empty;

	public DateTime ExpiresAt { get; set; }
}