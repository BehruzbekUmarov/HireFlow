using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class User
{
	public long Id { get; set; } 

	public string Email { get; set; } 
	public string PasswordHash { get; set; } 
	public string FullName { get; set; }
	public string?  Bio { get; set; }
	public string?  Skills { get; set; }
	public string?  PortfolioUrl { get; set; }
	public int? YearsOfExperience { get; set; }
	public string? PhoneNumber { get; set; }        
	public string? ProfilePictureUrl { get; set; }
	public UserRole Role { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }       
	public DateTime? DeletedAt { get; set; }

	public Company? Company { get; set; }
	public List<JobApplication> JobApplications { get; set; }
	public List<FreelancerCv> Cvs { get; set; }
	public List<Message> SentMessages { get; set; } 
	public List<RefreshToken> RefreshTokens { get; set; }
	public List<PasswordResetToken> PasswordResetTokens { get; set; }

	public User() 
	{
		Email = string.Empty;
		PasswordHash = string.Empty;
		FullName = string.Empty;
		IsDeleted = false;
		CreatedAt = DateTime.UtcNow;
		JobApplications = [];
		Cvs = [];
		SentMessages = [];	
		RefreshTokens = [];
		PasswordResetTokens = [];
	} 
}
