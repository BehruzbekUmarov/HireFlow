using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Features.Admin.Dtos;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Services;

public class AdminService : IAdminService
{
	private readonly IAppDbContext _db;

	public AdminService(IAppDbContext db)
	{
		_db = db;
	}

	public async Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(int pageNumber, int pageSize)
	{
		var total = await _db.Users.CountAsync();
		      
		var items = await _db.Users
			.OrderByDescending(u => u.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(u => new UserSummaryDto
			{
				Id = u.Id,
				Email = u.Email,
				FullName = u.FullName,
				Role = u.Role.ToString(),
				CreatedAt = u.CreatedAt
			})
			.ToListAsync();

		return new PagedResult<UserSummaryDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = pageNumber,
			PageSize = pageSize
		};
	}

	public async Task<PagedResult<CompanySummaryDto>> GetAllCompaniesAsync(int pageNumber, int pageSize)
	{
		var total = await _db.Companies.CountAsync();

		var items = await _db.Companies
			.Include(c => c.User)
			.OrderByDescending(c => c.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(c => new CompanySummaryDto
			{
				Id = c.Id,
				Name = c.Name,
				OwnerEmail = c.User!.Email,
				IsApproved = c.IsApproved,
				JobCount = c.Jobs.Count,
				CreatedAt = c.CreatedAt
			})
			.ToListAsync();

		return new PagedResult<CompanySummaryDto>
		{
			Items = items,
			TotalCount = total,
			PageNumber = pageNumber,
			PageSize = pageSize
		};
	}

	public async Task ApproveCompanyAsync(long companyId)
	{
		var company = await _db.Companies.FindAsync(companyId)
			?? throw new NotFoundException("Company", companyId);

		company.IsApproved = true;
		await _db.SaveChangesAsync();
	}

	public async Task SuspendCompanyAsync(long companyId)
	{
		var company = await _db.Companies.FindAsync(companyId)
			?? throw new NotFoundException("Company", companyId);

		company.IsApproved = false;
		await _db.SaveChangesAsync();
	}
}
