using HireFlow.Application.DTOs.TalentCard;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.TalentCard.Queries.GetMyTalentCards;

public class GetMyTalentCardsQueryHandler
	: IRequestHandler<GetMyTalentCardsQuery, List<TalentCardDto>>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetMyTalentCardsQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<List<TalentCardDto>> Handle(
		GetMyTalentCardsQuery query, CancellationToken cancellationToken)
	{
		return await _db.TalentCards
			.Include(t => t.User)
			.Where(t => t.UserId == _currentUser.UserId)
			.OrderByDescending(t => t.CreatedAt)
			.Select(t => new TalentCardDto
			{
				Id = t.Id,
				UserId = t.UserId,
				FreelancerName = t.User!.FullName,
				Title = t.Title,
				Description = t.Description,
				Category = t.Category,
				Skills = t.Skills,
				HourlyRate = t.HourlyRate,
				IsActive = t.IsActive,
				CreatedAt = t.CreatedAt,
				UpdatedAt = t.UpdatedAt
			})
			.ToListAsync(cancellationToken);
	}
}
