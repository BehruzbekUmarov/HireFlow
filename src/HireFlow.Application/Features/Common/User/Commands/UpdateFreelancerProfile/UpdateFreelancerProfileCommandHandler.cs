using HireFlow.Application.DTOs.User.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerProfile;

public sealed class UpdateFreelancerProfileCommandHandler
	: IRequestHandler<UpdateFreelancerProfileCommand, FreelancerProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateFreelancerProfileCommandHandler(
		IAppDbContext db,
		ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<FreelancerProfileDto> Handle(
		UpdateFreelancerProfileCommand command,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var user = await _db.Users
			.FirstOrDefaultAsync(
				u => u.Id == userId,
				cancellationToken)
			?? throw new NotFoundException(
				"User",
				userId);

		var request = command.Request;

		if (request.FullName is not null)
			user.FullName = request.FullName.Trim();

		if (request.Bio is not null)
			user.Bio = request.Bio.Trim();

		if (request.Skills is not null)
			user.Skills = request.Skills.Trim();

		if (request.PhoneNumber is not null)
			user.PhoneNumber = request.PhoneNumber.Trim();

		if (request.PortfolioUrl is not null)
			user.PortfolioUrl = request.PortfolioUrl.Trim();

		if (request.YearsOfExperience is not null)
			user.YearsOfExperience = request.YearsOfExperience;

		user.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		return new FreelancerProfileDto
		{
			Id = user.Id,
			Email = user.Email,
			FullName = user.FullName,
			Bio = user.Bio,
			Skills = user.Skills,
			PhoneNumber = user.PhoneNumber,
			PortfolioUrl = user.PortfolioUrl,
			YearsOfExperience = user.YearsOfExperience,
			CreatedAt = user.CreatedAt
		};
	}
}