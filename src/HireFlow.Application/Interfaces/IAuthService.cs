using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;

namespace HireFlow.Application.Interfaces;

public interface IAuthService
{
	Task<RegisterResponse> RegisterFreelancerAsync(RegisterFreelancerRequest request);
	Task<RegisterResponse> RegisterCompanyAsync(RegisterCompanyRequest request);
	Task<LoginResponse?> LoginAsync(LoginRequest request);
	Task<RefreshResponse?> RefreshAsync(string refreshToken);
}
