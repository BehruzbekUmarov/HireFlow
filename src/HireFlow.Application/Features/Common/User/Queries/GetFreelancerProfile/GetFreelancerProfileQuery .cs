using HireFlow.Application.DTOs.User;
using MediatR;

namespace HireFlow.Application.Features.Common.User.Queries.GetFreelancerProfile;

public record GetFreelancerProfileQuery : IRequest<FreelancerProfileDto>;