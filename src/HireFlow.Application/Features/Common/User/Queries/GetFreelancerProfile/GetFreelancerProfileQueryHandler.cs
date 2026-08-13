using HireFlow.Application.DTOs.User;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Queries.GetFreelancerProfile;

public class GetFreelancerProfileQueryHandler
	: IRequestHandler<GetFreelancerProfileQuery, FreelancerProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetFreelancerProfileQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<FreelancerProfileDto> Handle(
		GetFreelancerProfileQuery query, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var user = await _db.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
			?? throw new NotFoundException("User", userId);

		return new FreelancerProfileDto
		{
			Id = user.Id,
			Email = user.Email,
			FullName = user.FullName,
			Bio = user.Bio,
			Skills = user.Skills,
			PhoneNumber = user.PhoneNumber,
			PortfolioUrl = user.PortfolioUrl,
			ProfilePictureUrl = user.ProfilePictureUrl,
			YearsOfExperience = user.YearsOfExperience,
			CreatedAt = user.CreatedAt
		};
	}
}