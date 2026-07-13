using HireFlow.Application.DTOs.Auth.Requests;
using HireFlow.Application.DTOs.Auth.Responses;
using HireFlow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
		=> _authService = authService;

	[HttpPost("register/freelancer")]
	public async Task<ActionResult<RegisterResponse>> RegisterFreelancer(
	RegisterFreelancerRequest request)
	{
		var result = await _authService.RegisterFreelancerAsync(request);
		return Ok(result); // 200 — account created, go log in
	}

	[HttpPost("register/company")]
	public async Task<ActionResult<RegisterResponse>> RegisterCompany(
		RegisterCompanyRequest request)
	{
		var result = await _authService.RegisterCompanyAsync(request);
		return Ok(result);
	}

	[HttpPost("login")]
	[EnableRateLimiting("auth")]
	public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)   
	{
		var result = await _authService.LoginAsync(request);
		if (result is null)
			return Unauthorized(new { message = "Invalid email or password." });

		return Ok(result);
	}

	[HttpPost("refresh")]
	public async Task<ActionResult<RefreshResponse>> Refresh(RefreshRequest request)
	{
		var result = await _authService.RefreshAsync(request.RefreshToken);
		if (result is null)
			return Unauthorized(new { message = "Invalid or expired refresh token." });

		return Ok(result);
	}
}