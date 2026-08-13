using HireFlow.Application.DTOs.User;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerProfile;

public class UpdateFreelancerProfileCommandHandler
	: IRequestHandler<UpdateFreelancerProfileCommand, FreelancerProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateFreelancerProfileCommandHandler(
		IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<FreelancerProfileDto> Handle(
		UpdateFreelancerProfileCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var user = await _db.Users
			.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
			?? throw new NotFoundException("User", userId);

		var req = command.Request;

		if (req.Bio is not null) user.Bio = req.Bio.Trim();
		if (req.Skills is not null) user.Skills = req.Skills.Trim();
		if (req.PhoneNumber is not null) user.PhoneNumber = req.PhoneNumber.Trim();
		if (req.PortfolioUrl is not null) user.PortfolioUrl = req.PortfolioUrl.Trim();
		if (req.ProfilePictureUrl is not null) user.ProfilePictureUrl = req.ProfilePictureUrl.Trim();
		if (req.YearsOfExperience is not null) user.YearsOfExperience = req.YearsOfExperience;

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
			ProfilePictureUrl = user.ProfilePictureUrl,
			YearsOfExperience = user.YearsOfExperience,
			CreatedAt = user.CreatedAt
		};
	}
}
