using HireFlow.Application.DTOs.User.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Queries.GetCompanyProfile;

public sealed class GetCompanyProfileQueryHandler
	: IRequestHandler<GetCompanyProfileQuery, CompanyProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public GetCompanyProfileQueryHandler(
		IAppDbContext db,
		ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CompanyProfileDto> Handle(
		GetCompanyProfileQuery query,
		CancellationToken cancellationToken)
	{
		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException(
				"No company profile found.");

		var company = await _db.Companies
			.AsNoTracking()
			.Where(c => c.Id == companyId)
			.Select(c => new CompanyProfileDto
			{
				Id = c.Id,
				Name = c.Name,
				Description = c.Description,
				LogoUrl = c.LogoUrl,
				Website = c.Website,
				Location = c.Location,
				IsApproved = c.IsApproved,

				OwnerEmail = c.User!.Email,
				OwnerFullName = c.User.FullName,

				JobCount = c.Jobs.Count,

				CreatedAt = c.CreatedAt
			})
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException(
				"Company",
				companyId);

		return company;
	}
}