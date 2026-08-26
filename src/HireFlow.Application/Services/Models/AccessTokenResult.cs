namespace HireFlow.Application.Services.Models;

public sealed record AccessTokenResult(
	string Token,
	DateTime ExpiresAt);