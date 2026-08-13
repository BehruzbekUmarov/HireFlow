using HireFlow.Application.DTOs.User;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Queries.GetCompanyProfile;

public class GetCompanyProfileQueryHandler
	: IRequestHandler<GetCompanyProfileQuery, CompanyProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetCompanyProfileQueryHandler(IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CompanyProfileDto> Handle(
		GetCompanyProfileQuery query, CancellationToken cancellationToken)
	{
		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException("No company profile found.");

		var company = await _db.Companies
			.Include(c => c.User)
			.AsNoTracking()
			.FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken)
			?? throw new NotFoundException("Company", companyId);

		return MapToDto(company);
	}

	private static CompanyProfileDto MapToDto(Company company) => new()
	{
		Id = company.Id,
		Name = company.Name,
		Description = company.Description,
		LogoUrl = company.LogoUrl,
		Website = company.Website,
		Location = company.Location,
		IsApproved = company.IsApproved,
		OwnerEmail = company.User!.Email,
		OwnerFullName = company.User.FullName,
		JobCount = company.Jobs.Count,
		CreatedAt = company.CreatedAt
	};
}