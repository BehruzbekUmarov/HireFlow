using HireFlow.Application.DTOs.Auth.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshResponse?>;