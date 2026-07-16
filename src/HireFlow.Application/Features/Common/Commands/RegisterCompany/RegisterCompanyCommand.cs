using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;
using MediatR;

public sealed record RegisterCompanyCommand(RegisterCompanyRequest Request) : IRequest<RegisterResponse>;