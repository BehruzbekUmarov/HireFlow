using MediatR;

namespace HireFlow.Application.Features.JobApplication.Commands.WithdrawApplication;

public record WithdrawApplicationCommand(long ApplicationId) : IRequest;
