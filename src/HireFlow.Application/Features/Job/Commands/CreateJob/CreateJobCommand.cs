using HireFlow.Application.DTOs.Job.Requests;
using HireFlow.Application.DTOs.Job.Responses;
using HireFlow.Domain.Common;
using MediatR;

namespace HireFlow.Application.Features.Job.Commands.CreateJob;

public sealed record CreateJobCommand(CreateJobRequest Request) : IRequest<Result<JobDetailDto>>;
