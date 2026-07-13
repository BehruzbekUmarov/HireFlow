using HireFlow.Domain.Enums;

namespace HireFlow.Application.DTOs.Auth.Requests;

public class RegisterRequest
{
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public UserRole Role { get; set; }
}
