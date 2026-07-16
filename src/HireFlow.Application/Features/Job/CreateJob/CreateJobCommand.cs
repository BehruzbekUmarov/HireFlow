using HireFlow.Application.DTOs.Job;
using MediatR;

namespace HireFlow.Application.Features.Job.CreateJob;

public sealed record CreateJobCommand(CreateJobRequest Request) : IRequest<JobDetailDto>;
