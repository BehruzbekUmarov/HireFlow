using HireFlow.Application.DTOs.User.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.User.Queries.GetFreelancerProfile;

public record GetFreelancerProfileQuery : IRequest<FreelancerProfileDto>;