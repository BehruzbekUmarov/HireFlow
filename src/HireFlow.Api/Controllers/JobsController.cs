using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job;
using HireFlow.Application.Interfaces;
using HireFlow.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
	private readonly IJobService _jobService;

	public JobsController(IJobService jobService)
		=> _jobService = jobService;

	// GET api/jobs?keyword=developer&category=Backend&pageNumber=1
	// Public — anyone can browse jobs
	[HttpGet]
	public async Task<ActionResult<PagedResult<JobSummaryDto>>> Search([FromQuery] JobFilterRequest filter)
	{
		var result = await _jobService.SearchAsync(filter);
		return Ok(result);
	}

	// GET api/jobs/5
	[HttpGet("{id}")]
	public async Task<ActionResult<JobDetailDto>> GetById(long id)
	{
		var result = await _jobService.GetByIdAsync(id);
		if (result is null) return NotFound(new { message = "Job not found." });
		return Ok(result);
	}

	// GET api/jobs/my — company sees their own listings
	[HttpGet("my")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<PagedResult<JobSummaryDto>>> GetMyJobs(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var companyId = GetCompanyId();
		if (companyId is null) return Forbid();

		var result = await _jobService.GetByCompanyAsync(companyId.Value, pageNumber, pageSize);
		return Ok(result);
	}

	// POST api/jobs — company creates a job
	[HttpPost]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobDetailDto>> Create(CreateJobRequest request)
	{
		var companyId = GetCompanyId();
		if (companyId is null) return Forbid();

		try
		{
			var result = await _jobService.CreateAsync(companyId.Value, request);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	// PUT api/jobs/5 — company updates their job
	[HttpPut("{id}")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobDetailDto>> Update(long id, UpdateJobRequest request)
	{
		var companyId = GetCompanyId();
		if (companyId is null) return Forbid();

		try
		{
			var result = await _jobService.UpdateAsync(id, companyId.Value, request);
			return Ok(result);
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	// PATCH api/jobs/5/close — company closes a job listing
	[HttpPatch("{id}/close")]
	[Authorize(Roles = "Company")]
	public async Task<IActionResult> Close(long id)
	{
		var companyId = GetCompanyId();
		if (companyId is null) return Forbid();

		try
		{
			await _jobService.CloseAsync(id, companyId.Value);
			return NoContent();
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	// Helper — reads CompanyId claim from the JWT
	private long? GetCompanyId()
	{
		var claim = User.FindFirst("CompanyId")?.Value;
		return long.TryParse(claim, out var id) ? id : null;
	}
}
