using HireFlow.Domain.Entities;

namespace HireFlow.Application.Interfaces;

public interface ITokenService
{
	string GenerateAccessToken(User user);
	string GenerateRefreshToken();
	string HashToken(string token);

}
