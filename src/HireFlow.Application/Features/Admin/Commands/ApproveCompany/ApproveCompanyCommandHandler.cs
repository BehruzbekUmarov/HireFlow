using HireFlow.Domain.Exceptions;
using HireFlow.Domain.Interfaces;
using MediatR;

namespace HireFlow.Application.Features.Admin.Commands.ApproveCompany;

public sealed class ApproveCompanyCommandHandler : IRequestHandler<ApproveCompanyCommand>
{
	private readonly IAppDbContext _db;

	public ApproveCompanyCommandHandler(IAppDbContext db)
	{
		_db = db;
	}
	public async Task Handle(ApproveCompanyCommand request, CancellationToken cancellationToken)
	{
		var company = await _db.Companies.FindAsync(request.CompanyId, cancellationToken)
			?? throw new NotFoundException("Company", request.CompanyId);

		company.IsApproved = true;
		await _db.SaveChangesAsync(cancellationToken);	
	}
}
