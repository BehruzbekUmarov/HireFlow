using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Events;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.Commands.ForgetPassword;
public class ForgotPasswordCommandHandler
	: IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
	private readonly IAppDbContext _db;
	private readonly ITokenService _tokenService;
	private readonly IPublishEndpoint _publishEndpoint;

	public ForgotPasswordCommandHandler(
		IAppDbContext db,
		ITokenService tokenService,
		IPublishEndpoint publishEndpoint)
	{
		_db = db;
		_tokenService = tokenService;
		_publishEndpoint = publishEndpoint;
	}

	public async Task<ForgotPasswordResponse> Handle(
		ForgotPasswordCommand command, CancellationToken cancellationToken)
	{
		var email = command.Request.Email.ToLowerInvariant();
		var user = await _db.Users
			.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

		if (user is null)
			return new ForgotPasswordResponse
			{
				Message = "If that email is registered, a reset link has been sent."
			};

		var existing = await _db.PasswordResetTokens
			.Where(t => t.UserId == user.Id && !t.Used && t.ExpiresAt > DateTime.UtcNow)
			.ToListAsync(cancellationToken);

		foreach (var token in existing)
			token.Used = true;

		//var rawToken = _tokenService.GenerateRefreshToken(); 
		var code = GenerateCode();

		var tokenHash = _tokenService.HashToken(code);

		_db.PasswordResetTokens.Add(new PasswordResetToken
		{
			UserId = user.Id,
			TokenHash = tokenHash,
			ExpiresAt = DateTime.UtcNow.AddMinutes(15), 
			Used = false,
			CreatedAt = DateTime.UtcNow
		});

		await _db.SaveChangesAsync(cancellationToken);

		await _publishEndpoint.Publish(new PasswordResetRequestedEvent
		{
			ToEmail = user.Email,
			FullName = user.FullName,
			RawToken = code,
			ExpiresAt = DateTime.UtcNow.AddMinutes(15)
		}, cancellationToken);

		return new ForgotPasswordResponse
		{
			Message = "If that email is registered, a reset link has been sent."
		};
	}

	private static string GenerateCode()
	{
		return Random.Shared.Next(100000, 999999).ToString();
	}
}