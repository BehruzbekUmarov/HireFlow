namespace HireFlow.Domain.Entities;

public class Company
{
	public long Id { get; set; }
	public long UserId { get; set; }

	public string Name { get; set; } 
	public string Description { get; set; } 
	public string? LogoUrl { get; set; }     
	public string? Website { get; set; }   
	public string? Location { get; set; }
	public bool IsApproved { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; } 
	public DateTime? DeletedAt { get; set; }

	public User? User { get; set; }
	public List<Job> Jobs { get; set; }
	public Company()
	{
		Name = string.Empty;
		Description = string.Empty;
		IsApproved = false;
		IsDeleted = false;
		CreatedAt = DateTime.UtcNow;
		Jobs = [];
	}
}
