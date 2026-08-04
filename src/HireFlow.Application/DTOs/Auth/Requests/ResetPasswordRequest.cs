namespace HireFlow.Application.DTOs.Auth.Requests;

public class ResetPasswordRequest
{
	public string Code { get; set; } = string.Empty;
	public string NewPassword { get; set; } = string.Empty;
	public string ConfirmPassword { get; set; } = string.Empty;
}