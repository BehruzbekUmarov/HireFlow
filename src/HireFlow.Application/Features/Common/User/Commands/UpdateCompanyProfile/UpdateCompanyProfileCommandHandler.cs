using HireFlow.Application.DTOs.User;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;

public class UpdateCompanyProfileCommandHandler
	: IRequestHandler<UpdateCompanyProfileCommand, CompanyProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateCompanyProfileCommandHandler(
		IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CompanyProfileDto> Handle(
		UpdateCompanyProfileCommand command, CancellationToken cancellationToken)
	{
		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException("No company profile found.");

		var company = await _db.Companies
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken)
			?? throw new NotFoundException("Company", companyId);

		var req = command.Request;

		if (!string.IsNullOrWhiteSpace(req.Name))
			company.Name = req.Name.Trim();

		if (req.Description is not null)
			company.Description = req.Description.Trim();

		if (req.Website is not null)
			company.Website = req.Website.Trim();

		if (req.Location is not null)
			company.Location = req.Location.Trim();

		company.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

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