using MediatR;

namespace HireFlow.Application.Features.Common.User.Commands.UpdateCompanyLogo;

public record UpdateCompanyLogoCommand(string Url) : IRequest;
