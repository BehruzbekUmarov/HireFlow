using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Exceptions;
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

	[HttpPost("jobs/{jobId}/apply")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<JobApplicationDto>> Apply(
		long jobId, SubmitApplicationRequest request)
	{
		var userId = GetUserId();
		var result = await _applicationService.SubmitAsync(jobId, userId, request);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

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

	[HttpGet("jobs/{jobId}/applications")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<PagedResult<JobApplicationDto>>> GetByJob(
		long jobId,
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var companyId = GetCompanyId();
		var result = await _applicationService.GetByJobAsync(jobId, companyId, pageNumber, pageSize);
		return Ok(result);
	}

	[HttpGet("applications/{id}")]
	public async Task<ActionResult<JobApplicationDto>> GetById(long id)
	{
		var result = await _applicationService.GetByIdAsync(id);
		if (result is null) return NotFound(new { message = $"Application with id '{id}' was not found." });
		return Ok(result);
	}

	[HttpPatch("applications/{id}/status")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobApplicationDto>> ChangeStatus(
		long id, ChangeStatusRequest request)
	{
		var companyId = GetCompanyId();
		var result = await _applicationService.ChangeStatusAsync(id, companyId, request.NewStatus);
		return Ok(result);
	}

	private long GetUserId()
		=> long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

	private long GetCompanyId()
	{
		var claim = User.FindFirst("CompanyId")?.Value;
		if (!long.TryParse(claim, out var id))
			throw new ForbiddenException("Company profile not found in token.");
		return id;
	}
}
