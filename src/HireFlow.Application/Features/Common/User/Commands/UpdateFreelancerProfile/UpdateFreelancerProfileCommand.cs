using HireFlow.Application.DTOs.User;
using MediatR;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerProfile;

public record UpdateFreelancerProfileCommand(
	UpdateFreelancerProfileRequest Request) : IRequest<FreelancerProfileDto>;
