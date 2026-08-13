namespace HireFlow.Application.DTOs.User;

public class FreelancerProfileDto
{
	public long Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string? Bio { get; set; }
	public string? Skills { get; set; }
	public string? PhoneNumber { get; set; }
	public string? PortfolioUrl { get; set; }
	public string? ProfilePictureUrl { get; set; }
	public string? CvUrl { get; set; }
	public int? YearsOfExperience { get; set; }
	public DateTime CreatedAt { get; set; }
}
