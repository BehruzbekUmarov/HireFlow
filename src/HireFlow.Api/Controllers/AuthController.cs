using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Features.Common.Commands.ForgetPassword;
using HireFlow.Application.Features.Common.Commands.Login;
using HireFlow.Application.Features.Common.Commands.RefreshToken;
using HireFlow.Application.Features.Common.Commands.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;

	public AuthController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("register/freelancer")]
	public async Task<ActionResult<RegisterResponse>> RegisterFreelancer(RegisterFreelancerRequest request)
	{
		var result = await _mediator.Send(new RegisterFreelancerCommand(request));
		return Ok(result);
	}

	[HttpPost("register/company")]
	public async Task<ActionResult<RegisterResponse>> RegisterCompany(RegisterCompanyRequest request)
	{
		var result = await _mediator.Send(new RegisterCompanyCommand(request));
		return Ok(result);
	}

	[HttpPost("login")]
	public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
	{
		var result = await _mediator.Send(new LoginCommand(request));
		if (result is null)
			return Unauthorized(new { Message = "Invalid credentials." });

		return Ok(result);
	}

	[HttpPost("refresh")]
	public async Task<ActionResult<RefreshResponse>> Refresh(RefreshRequest request)
	{
		var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));
		if (result is null)
			return Unauthorized(new { Message = "Invalid or expired refresh token." });

		return Ok(result);
	}

	[HttpPost("forgot-password")]
	public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(
	ForgotPasswordRequest request)
	{
		var result = await _mediator.Send(new ForgotPasswordCommand(request));
		return Ok(result);
	}

	[HttpPost("reset-password")]
	public async Task<ActionResult<ResetPasswordResponse>> ResetPassword(
		ResetPasswordRequest request)
	{
		var result = await _mediator.Send(new ResetPasswordCommand(request));
		return Ok(result);
	}
}