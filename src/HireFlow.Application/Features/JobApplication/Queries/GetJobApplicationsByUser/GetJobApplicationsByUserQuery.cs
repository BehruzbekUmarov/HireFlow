using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication.Responses;
using MediatR;

namespace HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationsByUser;

public sealed record GetJobApplicationsByUserQuery(
	int PageNumber,
	int PageSize) : IRequest<PagedResult<JobApplicationDto>>;
