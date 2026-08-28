using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler
	: IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
	private readonly IAppDbContext _db;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ITokenService _tokenService;

	public ResetPasswordCommandHandler(
		IAppDbContext db,
		IPasswordHasher passwordHasher,
		ITokenService tokenService)
	{
		_db = db;
		_passwordHasher = passwordHasher;
		_tokenService = tokenService;
	}

	public async Task<ResetPasswordResponse> Handle(
		ResetPasswordCommand command,
		CancellationToken cancellationToken)
	{
		var request = command.Request;

		if (request.NewPassword != request.ConfirmPassword)
		{
			throw new InvalidOperationDomainException(
				"Passwords do not match.");
		}

		var email = request.Email
			.Trim()
			.ToLowerInvariant();

		var tokenHash = _tokenService.HashToken(request.Code);
		var now = DateTime.UtcNow;

		var resetToken = await _db.PasswordResetTokens
			.Include(t => t.User)
			.FirstOrDefaultAsync(
				t => 
					t.User!.Email == email &&
					t.TokenHash == tokenHash,
				cancellationToken);

		if (resetToken is null ||
			resetToken.Used ||
			resetToken.ExpiresAt <= now)
		{
			throw new InvalidOperationDomainException(
				"Reset code is invalid or has expired. Please request a new one.");
		}

		resetToken.User!.PasswordHash =
			_passwordHasher.Hash(request.NewPassword);

		resetToken.Used = true;

		var refreshTokens = await _db.RefreshTokens
			.Where(t =>
				t.UserId == resetToken.UserId &&
				!t.Revoked)
			.ToListAsync(cancellationToken);

		foreach (var token in refreshTokens)
		{
			token.Revoked = true;
		}

		await _db.SaveChangesAsync(cancellationToken);

		return new ResetPasswordResponse
		{
			Message =
				"Password reset successfully. Please log in with your new password."
		};
	}
}