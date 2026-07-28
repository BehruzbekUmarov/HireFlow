using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.Commands.ForgetPassword;

public record ForgotPasswordCommand(ForgotPasswordRequest Request)
	: IRequest<ForgotPasswordResponse>;
