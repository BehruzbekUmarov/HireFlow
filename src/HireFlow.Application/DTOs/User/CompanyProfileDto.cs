namespace HireFlow.Application.DTOs.User;

public class CompanyProfileDto
{
	public long Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? LogoUrl { get; set; }
	public string? Website { get; set; }
	public string? Location { get; set; }
	public bool IsApproved { get; set; }
	public string OwnerEmail { get; set; } = string.Empty;
	public string OwnerFullName { get; set; } = string.Empty;
	public int JobCount { get; set; }
	public DateTime CreatedAt { get; set; }
}