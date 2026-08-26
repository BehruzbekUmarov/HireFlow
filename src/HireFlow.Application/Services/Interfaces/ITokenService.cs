using HireFlow.Application.Services.Models;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Services.Interfaces;

public interface ITokenService
{
	AccessTokenResult GenerateAccessToken(User user);
	RefreshTokenResult GenerateRefreshToken();
	string HashToken(string token);

}
