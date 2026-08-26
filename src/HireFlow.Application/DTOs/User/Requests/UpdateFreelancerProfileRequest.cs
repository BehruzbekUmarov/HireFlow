namespace HireFlow.Application.DTOs.User.Requests;

public class UpdateFreelancerProfileRequest
{
	public string? FullName { get; set; }
	public string? Bio { get; set; }
	public string? Skills { get; set; }
	public string? PhoneNumber { get; set; }
	public string? PortfolioUrl { get; set; }
	public int? YearsOfExperience { get; set; }
}
