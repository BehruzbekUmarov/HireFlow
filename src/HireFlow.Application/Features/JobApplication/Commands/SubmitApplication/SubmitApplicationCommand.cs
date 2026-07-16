using HireFlow.Application.DTOs.JobApplication;
using MediatR;

public sealed record SubmitApplicationCommand(
	long JobId,
	SubmitApplicationRequest Request) : IRequest<JobApplicationDto>;