using HireFlow.Application.DTOs.Job;
using MediatR;

namespace HireFlow.Application.Features.Job.UpdateJob;

public sealed record UpdateJobCommand(long JobId, UpdateJobRequest Request) : IRequest<JobDetailDto>;