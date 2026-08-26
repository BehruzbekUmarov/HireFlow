using HireFlow.Application.DTOs.Job.Responses;
using MediatR;

namespace HireFlow.Application.Features.Job.Queries.GetJobById;

public sealed record GetJobByIdQuery(long JobId) : IRequest<JobDetailDto?>;