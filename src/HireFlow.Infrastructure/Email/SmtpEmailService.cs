using HireFlow.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace HireFlow.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<SmtpEmailService> _logger;

	public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public async Task SendApplicationStatusChangedAsync(
		string toEmail,
		string toName,
		string jobTitle,
		string companyName,
		string newStatus)
	{
		var subject = $"Your application for '{jobTitle}' has been updated";
		var body = BuildEmailBody(toName, jobTitle, companyName, newStatus);

		await SendAsync(toEmail, toName, subject, body);
	}

	private async Task SendAsync(string toEmail, string toName, string subject, string body)
	{
		var host = _configuration["Email:Host"]!;
		var port = _configuration.GetValue<int>("Email:Port", 587);
		var user = _configuration["Email:User"]!;
		var password = _configuration["Email:Password"]!;
		var from = _configuration["Email:From"] ?? user;

		using var client = new SmtpClient(host, port)
		{
			Credentials = new NetworkCredential(user, password),
			EnableSsl = true
		};

		using var message = new MailMessage
		{
			From = new MailAddress(from, "HireFlow"),
			Subject = subject,
			Body = body,
			IsBodyHtml = true
		};

		message.To.Add(new MailAddress(toEmail, toName));

		await client.SendMailAsync(message);
	}

	private static string BuildEmailBody(
		string name, string jobTitle, string companyName, string newStatus)
	{
		// Status-specific message
		var statusMessage = newStatus switch
		{
			"Reviewed" => "Your application is currently being reviewed by the company.",
			"Accepted" => "🎉 Congratulations! Your application has been accepted.",
			"Rejected" => "Unfortunately, your application was not selected this time.",
			_ => $"Your application status has been updated to: {newStatus}"
		};

		return $"""
            <html>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #3C3489;">Application Update — HireFlow</h2>
                <p>Hi <strong>{name}</strong>,</p>
                <p>{statusMessage}</p>
                <table style="border-collapse: collapse; width: 100%; margin: 20px 0;">
                    <tr>
                        <td style="padding: 8px; border: 1px solid #ddd; background: #f9f9f9;">
                            <strong>Job</strong>
                        </td>
                        <td style="padding: 8px; border: 1px solid #ddd;">{jobTitle}</td>
                    </tr>
                    <tr>
                        <td style="padding: 8px; border: 1px solid #ddd; background: #f9f9f9;">
                            <strong>Company</strong>
                        </td>
                        <td style="padding: 8px; border: 1px solid #ddd;">{companyName}</td>
                    </tr>
                    <tr>
                        <td style="padding: 8px; border: 1px solid #ddd; background: #f9f9f9;">
                            <strong>Status</strong>
                        </td>
                        <td style="padding: 8px; border: 1px solid #ddd;">
                            <strong style="color: #3C3489;">{newStatus}</strong>
                        </td>
                    </tr>
                </table>
                <p>Good luck with your job search!</p>
                <p style="color: #888; font-size: 12px;">— The HireFlow Team</p>
            </body>
            </html>
            """;
	}

	public async Task SendPasswordResetAsync(
	string toEmail, string toName, string rawToken, DateTime expiresAt)
	{
		// In production this would be your real frontend URL
		var resetUrl = $"https://hireflow.com/reset-password?token={rawToken}";

		var subject = "Reset your HireFlow password";
		var body = $"""
        <html>
        <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <h2 style="color: #3C3489;">Password Reset — HireFlow</h2>
            <p>Hi <strong>{toName}</strong>,</p>
            <p>We received a request to reset your password.
               Click the button below to set a new one:</p>
            <p style="text-align: center; margin: 30px 0;">
                <a href="{resetUrl}"
                   style="background: #3C3489; color: white; padding: 12px 24px;
                          text-decoration: none; border-radius: 6px; font-weight: bold;">
                    Reset Password
                </a>
            </p>
            <p style="color: #888; font-size: 13px;">
                This link expires at {expiresAt:HH:mm UTC, MMMM dd yyyy}.
                If you didn't request this, you can safely ignore this email.
            </p>
            <p style="color: #888; font-size: 12px;">— The HireFlow Team</p>
        </body>
        </html>
        """;

		await SendAsync(toEmail, toName, subject, body);
	}
}