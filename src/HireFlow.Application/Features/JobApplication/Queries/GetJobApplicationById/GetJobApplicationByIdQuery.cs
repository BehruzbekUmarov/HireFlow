using HireFlow.Application.DTOs.JobApplication;
using MediatR;

namespace HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationById;

public sealed record GetJobApplicationByIdQuery(long ApplicationId) : IRequest<JobApplicationDto?>;