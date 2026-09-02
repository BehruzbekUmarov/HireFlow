using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.TalentCard;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.TalentCard.Queries.SearchTalentCards;

public class SearchTalentCardsQueryHandler
	: IRequestHandler<SearchTalentCardsQuery, PagedResult<TalentCardDto>>
{
	private readonly IAppDbContext _db;

	public SearchTalentCardsQueryHandler(IAppDbContext db) => _db = db;

	public async Task<PagedResult<TalentCardDto>> Handle(
		SearchTalentCardsQuery query, CancellationToken cancellationToken)
	{
		var filter = query.Filter;

		var dbQuery = _db.TalentCards
			.Include(t => t.User)
			.AsNoTracking()
			.Where(t => t.IsActive)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(filter.Keyword))
			dbQuery = dbQuery.Where(t =>
				t.Title.Contains(filter.Keyword) ||
				t.Description.Contains(filter.Keyword) ||
				t.Skills.Contains(filter.Keyword));

		if (!string.IsNullOrWhiteSpace(filter.Category))
			dbQuery = dbQuery.Where(t => t.Category == filter.Category);

		if (filter.MinRate.HasValue)
			dbQuery = dbQuery.Where(t => t.HourlyRate >= filter.MinRate);

		if (filter.MaxRate.HasValue)
			dbQuery = dbQuery.Where(t => t.HourlyRate <= filter.MaxRate);

		var total = await dbQuery.CountAsync(cancellationToken);

		var items = await dbQuery
			.OrderByDescending(t => t.CreatedAt)
			.Skip((filter.PageNumber - 1) * filter.PageSize)
			.Take(filter.PageSize)
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

		return new PagedResult<TalentCardDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = filter.PageNumber,
			PageSize = filter.PageSize
		};
	}
}