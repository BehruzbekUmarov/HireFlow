using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Application.Features.Cv.Commands.CreateCv;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Commands.UpdateCv;

public class UpdateCvCommandHandler : IRequestHandler<UpdateCvCommand, CvDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateCvCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CvDto> Handle(
		UpdateCvCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;

		var cv = await _db.FreelancerCvs
			.FirstOrDefaultAsync(c => c.Id == command.CvId, cancellationToken)
			?? throw new NotFoundException("CV", command.CvId);

		if (cv.UserId != userId)
			throw new ForbiddenException("You can only edit your own CVs.");

		var req = command.Request;

		if (req.IsDefault && !cv.IsDefault)
		{
			var others = await _db.FreelancerCvs
				.Where(c => c.UserId == userId
						 && c.IsDefault
						 && c.Id != cv.Id)
				.ToListAsync(cancellationToken);

			foreach (var other in others)
				other.IsDefault = false;
		}

		cv.Title = req.Title.Trim();
		cv.Summary = req.Summary?.Trim();
		cv.Skills = req.Skills?.Trim();
		cv.Experience = req.Experience?.Trim();
		cv.Education = req.Education?.Trim();
		cv.Projects = req.Projects?.Trim();
		cv.Languages = req.Languages?.Trim();
		cv.PortfolioUrl = req.PortfolioUrl?.Trim();
		cv.YearsOfExperience = req.YearsOfExperience;
		cv.IsDefault = req.IsDefault;
		cv.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		return CreateCvCommandHandler.MapToDto(cv);
	}
}
