using System.ComponentModel.DataAnnotations;

namespace HireFlow.Application.Common.Configurations;

public sealed class JwtOptions
{
	public const string SectionName = nameof(JwtOptions);

	[Required]
	public required string Issuer { get; init; }

	[Required]
	public required string Audience { get; init; }

	[MinLength(32)]
	public required string SecretKey { get; init; } = default!;

	[Range(15, 120, ErrorMessage =
		"Access token expiration must be between 15 and 120 minutes")]
	public int AccessTokenMinutes { get; init; }

	[Range(7, 30, ErrorMessage =
		"Refresh token expiration must be between 7 day and 30 days")]
	public int RefreshTokenDays { get; set; }
}
