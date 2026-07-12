using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class JobApplicationsController : ControllerBase
{
	private readonly IJobApplicationService _applicationService;

	public JobApplicationsController(IJobApplicationService applicationService)
		=> _applicationService = applicationService;

	// POST api/jobs/5/apply — freelancer applies to a job
	[HttpPost("jobs/{jobId}/apply")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<JobApplicationDto>> Apply(long jobId, SubmitApplicationRequest request)
	{
		var userId = GetUserId();

		try
		{
			var result = await _applicationService.SubmitAsync(jobId, userId, request);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	// GET api/applications/my — freelancer sees their own applications
	[HttpGet("applications/my")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<PagedResult<JobApplicationDto>>> GetMine(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var userId = GetUserId();
		var result = await _applicationService.GetByUserAsync(userId, pageNumber, pageSize);
		return Ok(result);
	}

	// GET api/jobs/5/applications — company sees applicants for their job
	[HttpGet("jobs/{jobId}/applications")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<PagedResult<JobApplicationDto>>> GetByJob(
		long jobId,
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var companyId = GetCompanyId();
		if (companyId is null) return Forbid();

		try
		{
			var result = await _applicationService.GetByJobAsync(jobId, companyId.Value, pageNumber, pageSize);
			return Ok(result);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	// GET api/applications/5
	[HttpGet("applications/{id}")]
	public async Task<ActionResult<JobApplicationDto>> GetById(long id)
	{
		var result = await _applicationService.GetByIdAsync(id);
		if (result is null) return NotFound(new { message = "Application not found." });
		return Ok(result);
	}

	// PATCH api/applications/5/status — company changes application status
	[HttpPatch("applications/{id}/status")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobApplicationDto>> ChangeStatus(long id, ChangeStatusRequest request)
	{
		var companyId = GetCompanyId();
		if (companyId is null) return Forbid();

		try
		{
			var result = await _applicationService.ChangeStatusAsync(id, companyId.Value, request.NewStatus);
			return Ok(result);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	private long GetUserId()
		=> long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

	private long? GetCompanyId()
	{
		var claim = User.FindFirst("CompanyId")?.Value;
		return long.TryParse(claim, out var id) ? id : null;
	}
}
