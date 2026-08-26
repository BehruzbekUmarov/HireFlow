using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.Auth.Commands.Login;

public sealed class LoginCommandHandler
	: IRequestHandler<LoginCommand, LoginResponse?>
{
	private readonly IAppDbContext _db;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ITokenService _tokenService;

	public LoginCommandHandler(
		IAppDbContext db,
		IPasswordHasher passwordHasher,
		ITokenService tokenService)
	{
		_db = db;
		_passwordHasher = passwordHasher;
		_tokenService = tokenService;
	}

	public async Task<LoginResponse?> Handle(
		LoginCommand command,
		CancellationToken cancellationToken)
	{
		var email = command.Request.Email.ToLowerInvariant();

		var user = await _db.Users
			.FirstOrDefaultAsync(
				u => u.Email == email,
				cancellationToken);

		if (user is null ||
			!_passwordHasher.Verify(
				command.Request.Password,
				user.PasswordHash))
		{
			return null;
		}

		var accessToken = _tokenService.GenerateAccessToken(user);
		var refreshToken = _tokenService.GenerateRefreshToken();

		_db.RefreshTokens.Add(new HireFlow.Domain.Entities.RefreshToken
		{
			UserId = user.Id,
			TokenHash = _tokenService.HashToken(refreshToken.Token),
			ExpiresAt = refreshToken.ExpiresAt,
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
			AccessToken = accessToken.Token,
			RefreshToken = refreshToken.Token,
			AccessTokenExpiresAt = accessToken.ExpiresAt
		};
	}
}