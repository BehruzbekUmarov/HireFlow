using HireFlow.Application.DTOs.Auth;

namespace HireFlow.Application.Interfaces;

public interface IAuthService
{
	Task<AuthResponse> RegisterAsync(RegisterRequest request);
	Task<AuthResponse?> LoginAsync(LoginRequest request);
	Task<AuthResponse?> RefreshAsync(string refreshToken);
}
