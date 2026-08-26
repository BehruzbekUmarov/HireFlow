using System.ComponentModel.DataAnnotations;

namespace HireFlow.Application.Common.Configurations;

public sealed class JwtOptions
{
	public const string SectionName = nameof(JwtOptions);

	[Required]
	public string Issuer { get; init; } = string.Empty;

	[Required]
	public string Audience { get; init; } = string.Empty;

	[MinLength(32)]
	public string SecretKey { get; init; } = default!;

	[Range(15, 120, ErrorMessage =
		"Access token expiration must be between 15 and 120 minutes")]
	public int AccessTokenMinutes { get; init; }

	[Range(7, 30, ErrorMessage =
		"Refresh token expiration must be between 7 day and 30 days")]
	public int RefreshTokenDays { get; init; }
}
