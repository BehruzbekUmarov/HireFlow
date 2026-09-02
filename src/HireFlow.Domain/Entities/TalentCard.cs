namespace HireFlow.Domain.Entities;

public class TalentCard
{
	public long Id { get; set; }
	public long UserId { get; set; }

	public string Title { get; set; } = string.Empty;        // ".NET Backend Developer"
	public string Description { get; set; } = string.Empty;  // what they offer
	public string Category { get; set; } = string.Empty;     // "Backend", "Frontend"
	public decimal HourlyRate { get; set; }                  // price per hour
	public string Skills { get; set; } = string.Empty;       // "C#, .NET, PostgreSQL"
	public bool IsActive { get; set; } = true;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; set; }

	public User? User { get; set; }
}
