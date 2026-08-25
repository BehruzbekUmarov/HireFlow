using HireFlow.Application.Events;
using HireFlow.Application.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HireFlow.Infrastructure.Messaging;

public class ApplicationStatusChangedConsumer : IConsumer<ApplicationStatusChangedEvent>
{
	private readonly IEmailService _emailService;
	private readonly ILogger<ApplicationStatusChangedConsumer> _logger;

	public ApplicationStatusChangedConsumer(
		IEmailService emailService,
		ILogger<ApplicationStatusChangedConsumer> logger)
	{
		_emailService = emailService;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<ApplicationStatusChangedEvent> context)
	{
		var evt = context.Message;

		_logger.LogInformation(
			"Sending status change email to {Email} — Job: {Job}, Status: {Old} → {New}",
			evt.FreelancerEmail, evt.JobTitle, evt.OldStatus, evt.NewStatus);

		try
		{
			await _emailService.SendApplicationStatusChangedAsync(
				evt.FreelancerEmail,
				evt.FreelancerFullName,
				evt.JobTitle,
				evt.CompanyName,
				evt.NewStatus);

			_logger.LogInformation("Email sent successfully to {Email}", evt.FreelancerEmail);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send email to {Email}", evt.FreelancerEmail);
			throw;
		}
	}
}