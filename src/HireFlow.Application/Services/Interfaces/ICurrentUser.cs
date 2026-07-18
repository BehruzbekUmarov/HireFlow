namespace HireFlow.Application.Services.Interfaces;

public interface ICurrentUser
{
	long UserId { get; }
	long? CompanyId { get; }
	string Email { get; }
	string Role { get; }
	bool IsAdmin { get; }
	bool IsCompany { get; }
	bool IsFreelancer { get; }
	bool IsAuthenticated { get; }
}
