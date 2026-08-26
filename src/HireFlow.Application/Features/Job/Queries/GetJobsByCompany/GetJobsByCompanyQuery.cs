using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job.Responses;
using MediatR;

namespace HireFlow.Application.Features.Job.Queries.GetJobsByCompany;

public sealed record GetJobsByCompanyQuery(long CompanyId, int PageNumber, int PageSize) : IRequest<PagedResult<JobSummaryDto>>;