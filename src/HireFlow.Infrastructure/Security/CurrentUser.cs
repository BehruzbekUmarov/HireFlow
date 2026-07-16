using HireFlow.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HireFlow.Infrastructure.Security;

public sealed class CurrentUser : ICurrentUser
{
	public long UserId { get; }
	public long? CompanyId { get; }
	public string Email { get; } = string.Empty;
	public string Role { get; } = string.Empty;
	public bool IsAdmin { get; }
	public bool IsCompany { get; }
	public bool IsFreelancer { get; }
	public bool IsAuthenticated { get; }

	public CurrentUser(IHttpContextAccessor httpContextAccessor)
	{
		var user = httpContextAccessor.HttpContext?.User;

		if (user is null || user.Identity?.IsAuthenticated != true)
		{
			IsAuthenticated = false;
			return;
		}

		IsAuthenticated = true;

		var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (long.TryParse(nameIdentifierClaim, out var userId))
		{
			UserId = userId;
		}

		Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

		Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

		IsAdmin = Role == "Admin";
		IsCompany = Role == "Company";
		IsFreelancer = Role == "Freelancer";

		var companyIdClaim = user.FindFirst("CompanyId")?.Value;
		if (long.TryParse(companyIdClaim, out var companyId))
		{
			CompanyId = companyId;
		}
	}
}
