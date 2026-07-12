using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HireFlow.Infrastructure.Security;

public class TokenService : ITokenService
{
	private readonly string _secret;
	private readonly string _issuer;
	private readonly string _audience;
	private readonly int _accessTokenMinutes;

	public TokenService(IConfiguration configuration)
	{
		_secret = configuration["Jwt:Secret"]!;
		_issuer = configuration["Jwt:Issuer"]!;
		_audience = configuration["Jwt:Audience"]!;
		_accessTokenMinutes = configuration.GetValue<int>("Jwt:AccessTokenMinutes", 15);
	}

	public string GenerateAccessToken(User user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Email, user.Email),
			new(ClaimTypes.Role, user.Role.ToString()),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		if (user.Company is not null)
			claims.Add(new Claim("CompanyId", user.Company.Id.ToString()));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _issuer,
			audience: _audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public string GenerateRefreshToken()
		=> Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

	public string HashToken(string token)
		=> Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
