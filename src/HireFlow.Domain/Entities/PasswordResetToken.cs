namespace HireFlow.Domain.Entities;

public class PasswordResetToken
{
	public long Id { get; set; }
	public long UserId { get; set; }
	public string TokenHash { get; set; } = string.Empty;
	public DateTime ExpiresAt { get; set; }
	public bool Used { get; set; } = false;
	public DateTime CreatedAt { get; set; }

	public User? User { get; set; }

	public PasswordResetToken()
	{
		TokenHash = string.Empty;
		Used = false;
		CreatedAt = DateTime.UtcNow;
	}
}
