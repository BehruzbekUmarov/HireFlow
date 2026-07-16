using MediatR;

namespace HireFlow.Application.Features.Job.Commands.CloseJob;

public sealed record CloseJobCommand(long JobId) : IRequest;
