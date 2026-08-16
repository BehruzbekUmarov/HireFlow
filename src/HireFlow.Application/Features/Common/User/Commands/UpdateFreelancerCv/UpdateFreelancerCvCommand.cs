using MediatR;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerCv;

public record UpdateFreelancerCvCommand(string Url) : IRequest;