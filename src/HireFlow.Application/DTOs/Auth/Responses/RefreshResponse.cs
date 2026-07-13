namespace HireFlow.Application.DTOs.Auth.Responses;

public class RefreshResponse
{
	public string AccessToken { get; set; } = string.Empty;
	public string RefreshToken { get; set; } = string.Empty;
	public DateTime AccessTokenExpiresAt { get; set; }
}