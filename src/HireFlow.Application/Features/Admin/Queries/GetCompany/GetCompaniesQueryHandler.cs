using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Features.Admin.Dtos;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Admin.Queries.GetCompany;

public sealed class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, PagedResult<CompanySummaryDto>>
{
	private readonly IAppDbContext _db;

	public GetCompaniesQueryHandler(IAppDbContext db)
	{
		_db = db;
	}
	public async Task<PagedResult<CompanySummaryDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
	{
		var query = _db.Companies.AsNoTracking();

		var total = await query.CountAsync(cancellationToken);

		var items = await query 
			.OrderByDescending(c => c.CreatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(c => new CompanySummaryDto
			{
				Id = c.Id,
				Name = c.Name,
				OwnerEmail = c.User!.Email,
				IsApproved = c.IsApproved,
				JobCount = c.Jobs.Count,
				CreatedAt = c.CreatedAt
			})
			.ToListAsync(cancellationToken);

		return new PagedResult<CompanySummaryDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = request.PageNumber,
			PageSize = request.PageSize
		};
	}
}
