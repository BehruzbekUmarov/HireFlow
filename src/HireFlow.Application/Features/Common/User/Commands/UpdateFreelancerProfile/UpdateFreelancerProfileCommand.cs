using HireFlow.Application.DTOs.User.Requests;
using HireFlow.Application.DTOs.User.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerProfile;

public record UpdateFreelancerProfileCommand(
	UpdateFreelancerProfileRequest Request) : IRequest<FreelancerProfileDto>;
