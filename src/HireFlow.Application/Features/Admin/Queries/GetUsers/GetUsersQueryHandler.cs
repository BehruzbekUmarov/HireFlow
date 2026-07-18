using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Features.Admin.Dtos;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Admin.Queries.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserSummaryDto>>
{
	private readonly IAppDbContext _db;

	public GetUsersQueryHandler(IAppDbContext db)
	{
		_db = db;
	}
	public async Task<PagedResult<UserSummaryDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		var query = _db.Users.AsNoTracking();

		var total = await query.CountAsync(cancellationToken);

		var items = await _db.Users
			.AsNoTracking()
			.OrderByDescending(u => u.CreatedAt)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(u => new UserSummaryDto
			{
				Id = u.Id,
				Email = u.Email,
				FullName = u.FullName,
				Role = u.Role.ToString(),
				CreatedAt = u.CreatedAt
			})
			.ToListAsync(cancellationToken);

		return new PagedResult<UserSummaryDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = request.PageNumber,
			PageSize = request.PageSize
		};
	}
}
