namespace HireFlow.Domain.Entities;

public class Company
{
	public long Id { get; set; }
	public long UserId { get; set; }

	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public bool IsApproved { get; set; }
	public DateTime CreatedAt { get; set; }
	public User? User { get; set; }
	public List<Job> Jobs { get; set; }
	public Company()
	{
		Name = string.Empty;
		Description = string.Empty;
		IsApproved = false;
		CreatedAt = DateTime.UtcNow;
		Jobs = [];
	}
}
