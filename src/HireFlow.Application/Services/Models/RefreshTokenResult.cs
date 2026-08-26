namespace HireFlow.Application.Services.Models;

public sealed record RefreshTokenResult(
	string Token,
	DateTime ExpiresAt);
