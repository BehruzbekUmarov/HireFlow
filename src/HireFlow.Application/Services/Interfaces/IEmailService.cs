namespace HireFlow.Application.Services.Interfaces;

public interface IEmailService
{
	public Task SendApplicationStatusChangedAsync(
		string toEmail,
		string toName,
		string jobTitle,
		string companyName,
		string newStatus);

	public Task SendPasswordResetAsync(
		string toEmail,
		string toName,
		string rawToken,
		DateTime expiresAt);
}
