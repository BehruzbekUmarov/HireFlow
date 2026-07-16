using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.Job;
using HireFlow.Application.Features.Job.Commands.CloseJob;
using HireFlow.Application.Features.Job.CreateJob;
using HireFlow.Application.Features.Job.Queries.GetJobById;
using HireFlow.Application.Features.Job.Queries.GetJobsByCompany;
using HireFlow.Application.Features.Job.Queries.SearchJobs;
using HireFlow.Application.Features.Job.UpdateJob;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
	private readonly IMediator _mediator;

	public JobsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobDetailDto>> Create([FromBody] CreateJobRequest request)
	{
		var result = await _mediator.Send(new CreateJobCommand(request));
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id}")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobDetailDto>> Update(long id, [FromBody] UpdateJobRequest request)
	{
		var result = await _mediator.Send(new UpdateJobCommand(id, request));
		return Ok(result);
	}

	[HttpPatch("{id}/close")]
	[Authorize(Roles = "Company")]
	public async Task<IActionResult> Close(long id)
	{
		await _mediator.Send(new CloseJobCommand(id));
		return NoContent();
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<JobDetailDto>> GetById(long id)
	{
		var result = await _mediator.Send(new GetJobByIdQuery(id));
		if (result is null)
			return NotFound();

		return Ok(result);
	}

	[HttpGet("search")]
	public async Task<ActionResult<PagedResult<JobSummaryDto>>> Search([FromQuery] JobFilterRequest filter)
	{
		var result = await _mediator.Send(new SearchJobsQuery(filter));
		return Ok(result);
	}

	[HttpGet("company/{companyId}")]
	public async Task<ActionResult<PagedResult<JobSummaryDto>>> GetByCompany(
		long companyId,
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var result = await _mediator.Send(new GetJobsByCompanyQuery(companyId, pageNumber, pageSize));
		return Ok(result);
	}
}