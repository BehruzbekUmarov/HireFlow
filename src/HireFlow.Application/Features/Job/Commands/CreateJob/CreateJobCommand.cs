using HireFlow.Application.DTOs.Job.Requests;
using HireFlow.Application.DTOs.Job.Responses;
using MediatR;

namespace HireFlow.Application.Features.Job.Commands.CreateJob;

public sealed record CreateJobCommand(CreateJobRequest Request) : IRequest<JobDetailDto>;
