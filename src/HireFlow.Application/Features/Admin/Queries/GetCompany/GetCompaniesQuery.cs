using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Features.Admin.Dtos;
using MediatR;

namespace HireFlow.Application.Features.Admin.Queries.GetCompany;

public sealed record GetCompaniesQuery(int PageNumber, int PageSize) : IRequest<PagedResult<CompanySummaryDto>>;
