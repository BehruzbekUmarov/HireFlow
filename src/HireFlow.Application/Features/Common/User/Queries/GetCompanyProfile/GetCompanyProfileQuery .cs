using HireFlow.Application.DTOs.User.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.User.Queries.GetCompanyProfile;

public record GetCompanyProfileQuery : IRequest<CompanyProfileDto>;