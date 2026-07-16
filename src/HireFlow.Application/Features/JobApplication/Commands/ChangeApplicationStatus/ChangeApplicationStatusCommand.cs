using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Domain.Enums;
using MediatR;

namespace HireFlow.Application.Features.JobApplication.Commands.ChangeApplicationStatus;

public record ChangeApplicationStatusCommand(
	long ApplicationId,
	ApplicationStatus NewStatus) : IRequest<JobApplicationDto>;
