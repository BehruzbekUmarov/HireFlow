using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Commands.CreateCv;

public class CreateCvCommandHandler : IRequestHandler<CreateCvCommand, CvDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public CreateCvCommandHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CvDto> Handle(
		CreateCvCommand command, CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;
		var req = command.Request;

		if (req.IsDefault)
			await RemoveExistingDefaultAsync(userId, cancellationToken);

		var cv = new FreelancerCv
		{
			UserId = userId,
			Title = req.Title.Trim(),
			Summary = req.Summary?.Trim(),
			Skills = req.Skills?.Trim(),
			Experience = req.Experience?.Trim(),
			Education = req.Education?.Trim(),
			Languages = req.Languages?.Trim(),
			PortfolioUrl = req.PortfolioUrl?.Trim(),
			YearsOfExperience = req.YearsOfExperience,
			IsDefault = req.IsDefault,
			CreatedAt = DateTime.UtcNow
		};

		_db.FreelancerCvs.Add(cv);
		await _db.SaveChangesAsync(cancellationToken);

		return MapToDto(cv);
	}

	private async Task RemoveExistingDefaultAsync(
		long userId, CancellationToken ct)
	{
		var existing = await _db.FreelancerCvs
			.Where(c => c.UserId == userId && c.IsDefault)
			.ToListAsync(ct);

		foreach (var c in existing)
			c.IsDefault = false;
	}

	public static CvDto MapToDto(FreelancerCv cv) => new()
	{
		Id = cv.Id,
		Title = cv.Title,
		Summary = cv.Summary,
		Skills = cv.Skills,
		Experience = cv.Experience,
		Education = cv.Education,
		Languages = cv.Languages,
		PortfolioUrl = cv.PortfolioUrl,
		YearsOfExperience = cv.YearsOfExperience,
		FileUrl = cv.FileUrl,
		IsDefault = cv.IsDefault,
		CreatedAt = cv.CreatedAt,
		UpdatedAt = cv.UpdatedAt
	};
}
