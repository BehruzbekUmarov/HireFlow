using MediatR;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerAvatar;

public record UpdateFreelancerAvatarCommand(string Url) : IRequest;