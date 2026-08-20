using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Cv.Queries.GetMyCvs;

public class GetMyCvsQueryHandler : IRequestHandler<GetMyCvsQuery, List<CvDto>>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetMyCvsQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<List<CvDto>> Handle(
		GetMyCvsQuery query, CancellationToken cancellationToken)
	{
		return await _db.FreelancerCvs
			.Where(c => c.UserId == _currentUser.UserId)
			.OrderByDescending(c => c.IsDefault)
			.ThenByDescending(c => c.CreatedAt)
			.Select(c => new CvDto
			{
				Id = c.Id,
				Title = c.Title,
				Summary = c.Summary,
				Skills = c.Skills,
				Experience = c.Experience,
				Education = c.Education,
				Languages = c.Languages,
				PortfolioUrl = c.PortfolioUrl,
				YearsOfExperience = c.YearsOfExperience,
				FileUrl = c.FileUrl,
				IsDefault = c.IsDefault,
				CreatedAt = c.CreatedAt,
				UpdatedAt = c.UpdatedAt
			})
			.ToListAsync(cancellationToken);
	}
}
