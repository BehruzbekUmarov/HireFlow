using HireFlow.Domain.Entities;

namespace HireFlow.Application.Services.Interfaces;

public interface ITokenService
{
	string GenerateAccessToken(User user);
	string GenerateRefreshToken();
	string HashToken(string token);

}
