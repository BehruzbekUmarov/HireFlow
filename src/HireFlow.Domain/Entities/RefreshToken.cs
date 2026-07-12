namespace HireFlow.Domain.Entities;

public class RefreshToken
{
	public long Id { get; set; } 
	public long UserId { get; set; }
	public string TokenHash { get; set; }
	public DateTime ExpiresAt { get; set; }
	public bool Revoked { get; set; } 
	public DateTime CreatedAt { get; set; } 
	public User? User { get; set; } 
	public RefreshToken() 
	{
		TokenHash = string.Empty;
		Revoked = false;
		CreatedAt = DateTime.UtcNow;
	}
}
