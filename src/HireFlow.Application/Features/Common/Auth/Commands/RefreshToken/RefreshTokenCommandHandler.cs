using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HireFlow.Domain.Entities;
using HireFlow.Application.Services.Interfaces;

namespace HireFlow.Application.Features.Common.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshResponse?>
{
	private readonly IAppDbContext _db;
	private readonly ITokenService _tokenService;
	private readonly int _refreshTokenDays;
	private readonly int _accessTokenMinutes;

	public RefreshTokenCommandHandler(
		IAppDbContext db,
		ITokenService tokenService,
		IConfiguration configuration)
	{
		_db = db;
		_tokenService = tokenService;
		_refreshTokenDays = configuration.GetValue<int>("Jwt:RefreshTokenDays", 7);
		_accessTokenMinutes = configuration.GetValue<int>("Jwt:AccessTokenMinutes", 15);
	}

	public async Task<RefreshResponse?> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
	{
		var tokenHash = _tokenService.HashToken(command.RefreshToken);

		var stored = await _db.RefreshTokens
			.Include(t => t.User).ThenInclude(u => u!.Company)
			.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

		if (stored is null || stored.Revoked || stored.ExpiresAt < DateTime.UtcNow)
			return null;

		stored.Revoked = true;

		var accessToken = _tokenService.GenerateAccessToken(stored.User!);
		var rawRefreshToken = _tokenService.GenerateRefreshToken();

		_db.RefreshTokens.Add(new Domain.Entities.RefreshToken
		{
			UserId = stored.User!.Id,
			TokenHash = _tokenService.HashToken(rawRefreshToken),
			ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays),
			Revoked = false,
			CreatedAt = DateTime.UtcNow
		});

		await _db.SaveChangesAsync(cancellationToken);

		return new RefreshResponse
		{
			AccessToken = accessToken,
			RefreshToken = rawRefreshToken,
			AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_accessTokenMinutes)
		};
	}
}