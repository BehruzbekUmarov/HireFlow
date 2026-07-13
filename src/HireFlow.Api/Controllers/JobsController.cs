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

	[HttpGet]
	public async Task<ActionResult<PagedResult<JobSummaryDto>>> Search(
		[FromQuery] JobFilterRequest filter)
	{
		var result = await _jobService.SearchAsync(filter);
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<JobDetailDto>> GetById(long id)
	{
		var result = await _jobService.GetByIdAsync(id);
		if (result is null) return NotFound(new { message = $"Job with id '{id}' was not found." });
		return Ok(result);
	}

	[HttpGet("my")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<PagedResult<JobSummaryDto>>> GetMyJobs(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var companyId = GetCompanyId();
		var result = await _jobService.GetByCompanyAsync(companyId, pageNumber, pageSize);
		return Ok(result);
	}

	[HttpPost]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobDetailDto>> Create(CreateJobRequest request)
	{
		var companyId = GetCompanyId();
		var result = await _jobService.CreateAsync(companyId, request);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id}")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobDetailDto>> Update(long id, UpdateJobRequest request)
	{
		var companyId = GetCompanyId();
		var result = await _jobService.UpdateAsync(id, companyId, request);
		return Ok(result);
	}

	[HttpPatch("{id}/close")]
	[Authorize(Roles = "Company")]
	public async Task<IActionResult> Close(long id)
	{
		var companyId = GetCompanyId();
		await _jobService.CloseAsync(id, companyId);
		return NoContent();
	}

	private long GetCompanyId()
	{
		var claim = User.FindFirst("CompanyId")?.Value;
		if (!long.TryParse(claim, out var id))
			throw new Domain.Exceptions.ForbiddenException(
				"Company profile not found in token.");
		return id;
	}
}