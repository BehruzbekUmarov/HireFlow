using HireFlow.Application.DTOs.User.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Queries.GetFreelancerProfile;

public sealed class GetFreelancerProfileQueryHandler
	: IRequestHandler<GetFreelancerProfileQuery, FreelancerProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetFreelancerProfileQueryHandler(
		IAppDbContext db,
		ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<FreelancerProfileDto> Handle(
		GetFreelancerProfileQuery query,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var profile = await _db.Users
			.AsNoTracking()
			.Where(u => u.Id == userId)
			.Select(u => new FreelancerProfileDto
			{
				Id = u.Id,
				Email = u.Email,
				FullName = u.FullName,
				Bio = u.Bio,
				Skills = u.Skills,
				PhoneNumber = u.PhoneNumber,
				PortfolioUrl = u.PortfolioUrl,
				ProfilePictureUrl = u.ProfilePictureUrl,
				YearsOfExperience = u.YearsOfExperience,
				CreatedAt = u.CreatedAt
			})
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException(
				"User",
				userId);

		return profile;
	}
}