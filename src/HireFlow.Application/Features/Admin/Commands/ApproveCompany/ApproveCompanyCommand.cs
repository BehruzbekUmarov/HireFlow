using MediatR;

namespace HireFlow.Application.Features.Admin.Commands.ApproveCompany;

public sealed record ApproveCompanyCommand(long CompanyId) : IRequest;
