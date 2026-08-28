using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler
	: IRequestHandler<RefreshTokenCommand, RefreshResponse?>
{
	private readonly IAppDbContext _db;
	private readonly ITokenService _tokenService;

	public RefreshTokenCommandHandler(
		IAppDbContext db,
		ITokenService tokenService)
	{
		_db = db;
		_tokenService = tokenService;
	}

	public async Task<RefreshResponse?> Handle(
		RefreshTokenCommand command,
		CancellationToken cancellationToken)
	{
		var tokenHash = _tokenService.HashToken(command.RefreshToken);

		var stored = await _db.RefreshTokens
			.Include(t => t.User)
			.FirstOrDefaultAsync(
				t => t.TokenHash == tokenHash,
				cancellationToken);

		var now = DateTime.UtcNow;

		if (stored is null ||
			stored.Revoked ||
			stored.ExpiresAt <= now ||
			stored.User is null)
		{
			return null;
		}

		stored.Revoked = true;

		long? companyId = null;

		if (stored.User.Role == UserRole.Company)
		{
			companyId = await _db.Companies
				.Where(c => c.UserId == stored.User.Id)
				.Select(c => (long?)c.Id)
				.FirstOrDefaultAsync(cancellationToken);
		}

		var accessToken = _tokenService.GenerateAccessToken(
			stored.User,
			companyId);

		var refreshToken = _tokenService
			.GenerateRefreshToken();

		_db.RefreshTokens.Add(
			new Domain.Entities.RefreshToken
			{
				UserId = stored.User.Id,
				TokenHash =
					_tokenService.HashToken(refreshToken.Token),
				ExpiresAt = refreshToken.ExpiresAt,
				Revoked = false,
				CreatedAt = now
			});

		await _db.SaveChangesAsync(cancellationToken);

		return new RefreshResponse
		{
			AccessToken = accessToken.Token,
			RefreshToken = refreshToken.Token,
			AccessTokenExpiresAt = accessToken.ExpiresAt
		};
	}
}