using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;
using MediatR;

namespace HireFlow.Application.Features.Common.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(ResetPasswordRequest Request)
	: IRequest<ResetPasswordResponse>;