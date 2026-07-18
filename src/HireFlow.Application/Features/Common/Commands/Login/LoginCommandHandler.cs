using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HireFlow.Application.Features.Common.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse?>
{
	private readonly IAppDbContext _db;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ITokenService _tokenService;
	private readonly int _refreshTokenDays;
	private readonly int _accessTokenMinutes;

	public LoginCommandHandler(
		IAppDbContext db,
		IPasswordHasher passwordHasher,
		ITokenService tokenService,
		IConfiguration configuration)
	{
		_db = db;
		_passwordHasher = passwordHasher;
		_tokenService = tokenService;
		_refreshTokenDays = configuration.GetValue<int>("Jwt:RefreshTokenDays", 7);
		_accessTokenMinutes = configuration.GetValue<int>("Jwt:AccessTokenMinutes", 15);
	}

	public async Task<LoginResponse?> Handle(LoginCommand command, CancellationToken cancellationToken)
	{
		var request = command.Request;

		var user = await _db.Users
			.Include(u => u.Company)
			.FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

		if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
			return null;

		var accessToken = _tokenService.GenerateAccessToken(user);
		var rawRefreshToken = _tokenService.GenerateRefreshToken();

		_db.RefreshTokens.Add(new HireFlow.Domain.Entities.RefreshToken
		{
			UserId = user.Id,
			TokenHash = _tokenService.HashToken(rawRefreshToken),
			ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays),
			Revoked = false,
			CreatedAt = DateTime.UtcNow
		});

		await _db.SaveChangesAsync(cancellationToken);

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
}
