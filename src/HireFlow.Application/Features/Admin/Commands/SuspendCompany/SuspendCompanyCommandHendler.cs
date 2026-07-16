using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;

namespace HireFlow.Application.Features.Admin.Commands.SuspendCompany;

public sealed class SuspendCompanyCommandHendler : IRequestHandler<SuspendCompanyCommand>
{
	private readonly IAppDbContext _db;

	public SuspendCompanyCommandHendler(IAppDbContext db)
	{
		_db = db;
	}
	public async Task Handle(SuspendCompanyCommand request, CancellationToken cancellationToken)
	{
		var company = await _db.Companies.FindAsync(request.CompanyId, cancellationToken)
			?? throw new NotFoundException("Company", request.CompanyId);

		company.IsApproved = false;
		await _db.SaveChangesAsync(cancellationToken);
	}
}
