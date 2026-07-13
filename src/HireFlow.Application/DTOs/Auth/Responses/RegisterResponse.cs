namespace HireFlow.Application.DTOs.Auth.Responses;

public class RegisterResponse
{
	public long Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string Role { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public string Message { get; set; } = string.Empty;
}
