using HireFlow.Application.DTOs.JobApplication.Requests;
using HireFlow.Application.DTOs.JobApplication.Responses;
using MediatR;

public sealed record SubmitApplicationCommand(
	long JobId,
	SubmitApplicationRequest Request) : IRequest<JobApplicationDto>;