using HireFlow.Application.Services.Interfaces;
using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateCompanyLogo;

public class UpdateCompanyLogoCommandHandler
	: IRequestHandler<UpdateCompanyLogoCommand>
{
	private readonly IAppDbContext _db;
	private readonly ICurrentUser _currentUser;

	public UpdateCompanyLogoCommandHandler(
		IAppDbContext db, ICurrentUser currentUser)
	{
		_db = db;
		_currentUser = currentUser;
	}

	public async Task Handle(
		UpdateCompanyLogoCommand command, CancellationToken ct)
	{
		var companyId = _currentUser.CompanyId
			?? throw new ForbiddenException("No company profile found.");

		var company = await _db.Companies
			.FirstOrDefaultAsync(c => c.Id == companyId, ct)
			?? throw new NotFoundException("Company", companyId);

		company.LogoUrl = command.Url;
		company.UpdatedAt = DateTime.UtcNow;

		await _db.SaveChangesAsync(ct);
	}
}
