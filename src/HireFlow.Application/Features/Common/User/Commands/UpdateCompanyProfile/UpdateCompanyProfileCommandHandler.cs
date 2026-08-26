using HireFlow.Application.DTOs.User.Responses;
using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;

public sealed class UpdateCompanyProfileCommandHandler
	: IRequestHandler<UpdateCompanyProfileCommand, CompanyProfileDto>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateCompanyProfileCommandHandler(
		IAppDbContext db,
		ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task<CompanyProfileDto> Handle(
		UpdateCompanyProfileCommand command,
		CancellationToken cancellationToken)
	{
		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException(
				"No company profile found.");

		var company = await _db.Companies
			.Include(c => c.User)
			.FirstOrDefaultAsync(
				c => c.Id == companyId,
				cancellationToken)
			?? throw new NotFoundException(
				"Company",
				companyId);

		var request = command.Request;

		if (!string.IsNullOrWhiteSpace(request.Name))
		{
			company.Name = request.Name.Trim();
		}

		if (request.Description is not null)
		{
			company.Description = request.Description.Trim();
		}

		if (request.Website is not null)
		{
			company.Website = request.Website.Trim();
		}

		if (request.Location is not null)
		{
			company.Location = request.Location.Trim();
		}

		company.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(cancellationToken);

		var jobCount = await _db.Jobs
			.CountAsync(
				j => j.CompanyId == companyId,
				cancellationToken);

		return new CompanyProfileDto
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
			JobCount = jobCount,
			CreatedAt = company.CreatedAt
		};
	}
}