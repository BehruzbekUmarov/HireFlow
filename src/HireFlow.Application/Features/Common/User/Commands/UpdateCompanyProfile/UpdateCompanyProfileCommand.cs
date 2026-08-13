using HireFlow.Application.DTOs.User;
using MediatR;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;

public record UpdateCompanyProfileCommand(
	UpdateCompanyProfileRequest Request) : IRequest<CompanyProfileDto>;