using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
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
	private readonly int _accessTokenMinutes; // ← add this

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
		_accessTokenMinutes = configuration.GetValue<int>("Jwt:AccessTokenMinutes", 15); // ← add this
	}

	public async Task<RegisterResponse> RegisterFreelancerAsync(RegisterFreelancerRequest request)
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
			Role = UserRole.Freelancer,
			CreatedAt = DateTime.UtcNow,
			JobApplications = [],
			RefreshTokens = []
		};

		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		return new RegisterResponse
		{
			Id = user.Id,
			Email = user.Email,
			FullName = user.FullName,
			Role = user.Role.ToString(),
			CreatedAt = user.CreatedAt,
			Message = "Account created successfully. Please log in."
		};
	}

	public async Task<RegisterResponse> RegisterCompanyAsync(RegisterCompanyRequest request)
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
			Role = UserRole.Company,
			CreatedAt = DateTime.UtcNow,
			JobApplications = [],
			RefreshTokens = [],
			Company = new Company
			{
				Name = request.CompanyName.Trim(),
				IsApproved = false,
				CreatedAt = DateTime.UtcNow
			}
		};

		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		return new RegisterResponse
		{
			Id = user.Id,
			Email = user.Email,
			FullName = user.FullName,
			Role = user.Role.ToString(),
			CreatedAt = user.CreatedAt,
			Message = "Company account created. Pending admin approval before posting jobs."
		};
		// Notice: Company gets a different message — they know they need approval
	}

	public async Task<LoginResponse?> LoginAsync(LoginRequest request)
	{
		var user = await _db.Users
			.Include(u => u.Company)
			.FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant());

		if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
			return null;

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

		return new LoginResponse
		{
			Id = user.Id,
			Email = user.Email,
			FullName = user.FullName,
			Role = user.Role.ToString(),
			AccessToken = accessToken,
			RefreshToken = rawRefreshToken,
			AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes)
		};
	}

	public async Task<RefreshResponse?> RefreshAsync(string refreshToken)
	{
		var tokenHash = _tokenService.HashToken(refreshToken);

		var stored = await _db.RefreshTokens
			.Include(t => t.User).ThenInclude(u => u!.Company)
			.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

		if (stored is null || stored.Revoked || stored.ExpiresAt < DateTime.UtcNow)
			return null;

		stored.Revoked = true;

		var accessToken = _tokenService.GenerateAccessToken(stored.User!);
		var rawRefreshToken = _tokenService.GenerateRefreshToken();

		_db.RefreshTokens.Add(new RefreshToken
		{
			UserId = stored.User!.Id,
			TokenHash = _tokenService.HashToken(rawRefreshToken),
			ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays),
			Revoked = false,
			CreatedAt = DateTime.UtcNow
		});

		await _db.SaveChangesAsync();

		return new RefreshResponse
		{
			AccessToken = accessToken,
			RefreshToken = rawRefreshToken,
			AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes)
		};
	}
}
