using HireFlow.Application.DTOs.Admin;
using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]  // entire controller is Admin-only
public class AdminController : ControllerBase
{
	private readonly IAdminService _adminService;

	public AdminController(IAdminService adminService)
		=> _adminService = adminService;

	// GET api/admin/users
	[HttpGet("users")]
	public async Task<ActionResult<PagedResult<UserSummaryDto>>> GetUsers(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 20)
	{
		var result = await _adminService.GetAllUsersAsync(pageNumber, pageSize);
		return Ok(result);
	}

	// GET api/admin/companies
	[HttpGet("companies")]
	public async Task<ActionResult<PagedResult<CompanySummaryDto>>> GetCompanies(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 20)
	{
		var result = await _adminService.GetAllCompaniesAsync(pageNumber, pageSize);
		return Ok(result);
	}

	// PATCH api/admin/companies/5/approve
	[HttpPatch("companies/{id}/approve")]
	public async Task<IActionResult> ApproveCompany(long id)
	{
		try
		{
			await _adminService.ApproveCompanyAsync(id);
			return NoContent();
		}
		catch (InvalidOperationException ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}

	// PATCH api/admin/companies/5/suspend
	[HttpPatch("companies/{id}/suspend")]
	public async Task<IActionResult> SuspendCompany(long id)
	{
		try
		{
			await _adminService.SuspendCompanyAsync(id);
			return NoContent();
		}
		catch (InvalidOperationException ex)
		{
			return NotFound(new { message = ex.Message });
		}
	}
}
