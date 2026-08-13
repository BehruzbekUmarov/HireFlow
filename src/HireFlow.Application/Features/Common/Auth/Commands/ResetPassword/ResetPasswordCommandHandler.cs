using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler
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
		ResetPasswordCommand command, CancellationToken cancellationToken)
	{
		if (command.Request.NewPassword != command.Request.ConfirmPassword)
			throw new InvalidOperationDomainException("Passwords do not match.");

		var tokenHash = _tokenService.HashToken(command.Request.Code);

		var resetToken = await _db.PasswordResetTokens
			.Include(t => t.User)
			.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

		if (resetToken is null || resetToken.Used || resetToken.ExpiresAt < DateTime.UtcNow)
			throw new InvalidOperationDomainException(
				"Reset code is invalid or has expired. Please request a new one.");

		resetToken.User!.PasswordHash =
			_passwordHasher.Hash(command.Request.NewPassword);

		resetToken.Used = true;

		var refreshTokens = await _db.RefreshTokens
			.Where(t => t.UserId == resetToken.UserId && !t.Revoked)
			.ToListAsync(cancellationToken);

		foreach (var token in refreshTokens)
			token.Revoked = true;

		await _db.SaveChangesAsync(cancellationToken);

		return new ResetPasswordResponse
		{
			Message = "Password reset successfully. Please log in with your new password."
		};
	}
}