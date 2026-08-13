using MediatR;

namespace HireFlow.Application.Features.Job.Commands.DeleteJob;

public record AdminDeleteJobCommand(long JobId) : IRequest;
