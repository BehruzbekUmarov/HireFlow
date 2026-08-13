using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.Auth.Commands.Login;

public sealed record LoginCommand(LoginRequest Request) : IRequest<LoginResponse?>;
