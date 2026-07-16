using MediatR;

namespace HireFlow.Application.Features.Admin.Commands.SuspendCompany;

public sealed record SuspendCompanyCommand(long CompanyId) : IRequest;