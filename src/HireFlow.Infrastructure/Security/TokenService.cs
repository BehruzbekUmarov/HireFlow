using HireFlow.Application.Common.Configurations;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Application.Services.Models;
using HireFlow.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HireFlow.Infrastructure.Security;

public sealed class TokenService : ITokenService
{
	private readonly JwtOptions _options;

	public TokenService(IOptions<JwtOptions> options)
	{
		_options = options.Value;
	}

	public AccessTokenResult GenerateAccessToken(
	User user,
	long? companyId)
	{
		var now = DateTime.UtcNow;
		var expiresAt = now.AddMinutes(_options.AccessTokenMinutes);

		var claims = new List<Claim>
	{
		new(ClaimTypes.NameIdentifier, user.Id.ToString()),
		new(ClaimTypes.Email, user.Email),
		new(ClaimTypes.Role, user.Role.ToString()),
		new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
	};

		if (companyId.HasValue)
		{
			claims.Add(
				new Claim("CompanyId", companyId.Value.ToString()));
		}

		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(_options.SecretKey));

		var credentials = new SigningCredentials(
			key,
			SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: _options.Issuer,
			audience: _options.Audience,
			claims: claims,
			expires: expiresAt,
			signingCredentials: credentials);

		var tokenString = new JwtSecurityTokenHandler()
			.WriteToken(token);

		return new AccessTokenResult(
			tokenString,
			expiresAt);
	}

	public RefreshTokenResult GenerateRefreshToken()
	{
		var now = DateTime.UtcNow;
		var expiresAt = now.AddDays(_options.RefreshTokenDays);

		var token = Convert.ToBase64String(
			RandomNumberGenerator.GetBytes(64));

		return new RefreshTokenResult(token, expiresAt);
	}

	public string HashToken(string token)
	{
		return Convert.ToHexString(
			SHA256.HashData(
				Encoding.UTF8.GetBytes(token)));
	}
}
