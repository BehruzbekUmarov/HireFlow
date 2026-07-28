
using HireFlow.Application.Events;
using HireFlow.Application.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HireFlow.Application.RabitMQ;

public class PasswordResetRequestedConsumer : IConsumer<PasswordResetRequestedEvent>
{
	private readonly IEmailService _emailService;
	private readonly ILogger<PasswordResetRequestedConsumer> _logger;

	public PasswordResetRequestedConsumer(
		IEmailService emailService,
		ILogger<PasswordResetRequestedConsumer> logger)
	{
		_emailService = emailService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<PasswordResetRequestedEvent> context)
	{
		var evt = context.Message;

		_logger.LogInformation("Sending password reset email to {Email}", evt.ToEmail);

		await _emailService.SendPasswordResetAsync(
			evt.ToEmail,
			evt.FullName,
			evt.RawToken,
			evt.ExpiresAt);
	}
}