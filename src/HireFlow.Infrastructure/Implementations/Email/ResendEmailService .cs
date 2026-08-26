using HireFlow.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HireFlow.Infrastructure.Implementations.Email;

public class ResendEmailService : IEmailService
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ResendEmailService> _logger;
	private readonly string _fromAddress;

	public ResendEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<ResendEmailService> logger)
	{
		_httpClient = httpClient;
		_logger = logger;

		_httpClient.BaseAddress = new Uri("https://api.resend.com/");
		_httpClient.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", configuration["Resend:ApiKey"]);

		_fromAddress = configuration["Resend:From"] ?? "onboarding@resend.dev";
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

	public async Task SendPasswordResetAsync(
		string toEmail, string toName, string rawToken, DateTime expiresAt)
	{
		var subject = "Your HireFlow password reset code";
		var body = $"""
        <html>
        <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <h2 style="color: #3C3489;">Password Reset — HireFlow</h2>
            <p>Hi <strong>{toName}</strong>,</p>
            <p>Use this code to reset your password:</p>
            
            <div style="text-align: center; margin: 30px 0;">
                <span style="font-size: 42px; font-weight: bold; letter-spacing: 12px;
                             color: #3C3489; background: #EEEDFE; padding: 16px 24px;
                             border-radius: 12px; font-family: monospace;">
                    {rawToken}
                </span>
            </div>
            
            <p style="color: #888; font-size: 13px; text-align: center;">
                This code expires in <strong>15 minutes</strong>
                ({expiresAt:HH:mm UTC}).
            </p>
            <p style="color: #888; font-size: 13px;">
                If you didn't request this, you can safely ignore this email.
            </p>
            <p style="color: #888; font-size: 12px;">— The HireFlow Team</p>
        </body>
        </html>
        """;

		await SendAsync(toEmail, toName, subject, body);
	}

	private async Task SendAsync(string toEmail, string toName, string subject, string body)
	{
		var payload = new ResendEmailRequest
		{
			From = $"HireFlow <{_fromAddress}>",
			To = new[] { toEmail },
			Subject = subject,
			Html = body
		};

		var response = await _httpClient.PostAsJsonAsync("emails", payload);

		if (!response.IsSuccessStatusCode)
		{
			var error = await response.Content.ReadAsStringAsync();
			_logger.LogError("Resend email failed: {StatusCode} - {Error}", response.StatusCode, error);
			response.EnsureSuccessStatusCode();
		}
	}

	private static string BuildEmailBody(
		string name, string jobTitle, string companyName, string newStatus)
	{
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
}

public class ResendEmailRequest
{
	[JsonPropertyName("from")]
	public string From { get; set; } = string.Empty;

	[JsonPropertyName("to")]
	public string[] To { get; set; } = Array.Empty<string>();

	[JsonPropertyName("subject")]
	public string Subject { get; set; } = string.Empty;

	[JsonPropertyName("html")]
	public string Html { get; set; } = string.Empty;
}