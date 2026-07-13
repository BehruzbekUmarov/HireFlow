using HireFlow.Application.DTOs.Auth;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HireFlow.Application.Services;

public class AuthService : IAuthService
{
	private readonly IAppDbContext _db;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ITokenService _tokenService;
	private readonly int _refreshTokenDays;

	public AuthService(
		IAppDbContext db,
		IPasswordHasher passwordHasher,
		ITokenService tokenService,
		IConfiguration configuration)
	{
		_db = db;
		_passwordHasher = passwordHasher;
		_tokenService = tokenService;
		_refreshTokenDays = configuration.GetValue<int>("Jwt:RefreshTokenDays", 7);
	}

	public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
	{
		var emailTaken = await _db.Users
			.AnyAsync(u => u.Email == request.Email.ToLowerInvariant());

		if (emailTaken)
			throw new ConflictException("Email is already registered.");

		var user = new User
		{
			Email = request.Email.Trim().ToLowerInvariant(),
			PasswordHash = _passwordHasher.Hash(request.Password),
			FullName = request.FullName.Trim(),
			Role = request.Role,
			CreatedAt = DateTime.UtcNow
		};

		if (request.Role == Domain.Enums.UserRole.Company)
		{
			user.Company = new Company
			{
				Name = request.FullName.Trim(),
				IsApproved = false,
				CreatedAt = DateTime.UtcNow
			};
		}

		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		return await IssueTokensAsync(user);
	}

	public async Task<AuthResponse?> LoginAsync(LoginRequest request)
	{
		var user = await _db.Users
			.Include(u => u.Company)
			.FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant());

		if (user is null) return null;
		if (!_passwordHasher.Verify(request.Password, user.PasswordHash)) return null;

		return await IssueTokensAsync(user);
	}

	public async Task<AuthResponse?> RefreshAsync(string refreshToken)
	{
		var tokenHash = _tokenService.HashToken(refreshToken);

		var stored = await _db.RefreshTokens
			.Include(t => t.User).ThenInclude(u => u!.Company)
			.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

		if (stored is null || stored.Revoked || stored.ExpiresAt < DateTime.UtcNow)
			return null;

		stored.Revoked = true;
		await _db.SaveChangesAsync();

		return await IssueTokensAsync(stored.User!);
	}

	private async Task<AuthResponse> IssueTokensAsync(User user)
	{
		var accessToken = _tokenService.GenerateAccessToken(user);
		var rawRefreshToken = _tokenService.GenerateRefreshToken();

		_db.RefreshTokens.Add(new RefreshToken
		{
			UserId = user.Id,
			TokenHash = _tokenService.HashToken(rawRefreshToken),
			ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays),
			Revoked = false,
			CreatedAt = DateTime.UtcNow
		});

		await _db.SaveChangesAsync();

		return new AuthResponse
		{
			AccessToken = accessToken,
			RefreshToken = rawRefreshToken,
			AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
			FullName = user.FullName,
			Role = user.Role.ToString()
		};
	}
}
