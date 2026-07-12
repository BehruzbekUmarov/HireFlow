using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class User
{
	public long Id { get; set; } 

	public string Email { get; set; } 
	public string PasswordHash { get; set; } 
	public string FullName { get; set; } 
	public UserRole Role { get; set; }
	public DateTime CreatedAt { get; set; } 

	public Company? Company { get; set; }
	public List<JobApplication> JobApplications { get; set; } 
	public List<RefreshToken> RefreshTokens { get; set; } 

	public User() 
	{
		Email = string.Empty;
		PasswordHash = string.Empty;
		FullName = string.Empty;
		CreatedAt = DateTime.UtcNow;
		JobApplications = [];
		RefreshTokens = [];
	} 
}
