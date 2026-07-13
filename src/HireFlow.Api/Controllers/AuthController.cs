using HireFlow.Application.DTOs.Auth;
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

	[HttpPost("register")]
	public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
	{
		var result = await _authService.RegisterAsync(request);
		return Ok(result);
	}

	[HttpPost("login")]
	[EnableRateLimiting("auth")]
	public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
	{
		var result = await _authService.LoginAsync(request);
		if (result is null)
			return Unauthorized(new { message = "Invalid email or password." });

		return Ok(result);
	}

	[HttpPost("refresh")]
	public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
	{
		var result = await _authService.RefreshAsync(request.RefreshToken);
		if (result is null)
			return Unauthorized(new { message = "Invalid or expired refresh token." });

		return Ok(result);
	}
}