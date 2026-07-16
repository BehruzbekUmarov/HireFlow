using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Features.Admin.Dtos;
using MediatR;

namespace HireFlow.Application.Features.Admin.Queries.GetUsers;

public sealed record GetUsersQuery(int PageNumber, int PageSize) : IRequest<PagedResult<UserSummaryDto>>;