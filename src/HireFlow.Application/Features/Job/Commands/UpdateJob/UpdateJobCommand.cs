using HireFlow.Application.DTOs.Job.Requests;
using HireFlow.Application.DTOs.Job.Responses;
using MediatR;

namespace HireFlow.Application.Features.Job.Commands.UpdateJob;

public sealed record UpdateJobCommand(long JobId, UpdateJobRequest Request) : IRequest<JobDetailDto>;