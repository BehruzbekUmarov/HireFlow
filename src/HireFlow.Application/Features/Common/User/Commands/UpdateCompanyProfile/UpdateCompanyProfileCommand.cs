using HireFlow.Application.DTOs.User.Requests;
using HireFlow.Application.DTOs.User.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;

public record UpdateCompanyProfileCommand(
	UpdateCompanyProfileRequest Request) : IRequest<CompanyProfileDto>;