
using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication.Responses;
using MediatR;

namespace HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationByJob;

public sealed record GetJobApplicationsByJobQuery(
	long JobId,
	int PageNumber,
	int PageSize) : IRequest<PagedResult<JobApplicationDto>>;
