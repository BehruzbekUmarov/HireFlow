using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job.Requests;
using HireFlow.Application.DTOs.Job.Responses;
using MediatR;

namespace HireFlow.Application.Features.Job.Queries.SearchJobs;

public sealed record SearchJobsQuery(JobFilterRequest Filter) : IRequest<PagedResult<JobSummaryDto>>;
