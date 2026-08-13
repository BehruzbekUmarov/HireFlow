using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.JobApplication;
using HireFlow.Application.Features.JobApplication.Commands.ChangeApplicationStatus;
using HireFlow.Application.Features.JobApplication.Commands.WithdrawApplication;
using HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationById;
using HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationByJob;
using HireFlow.Application.Features.JobApplication.Queries.GetJobApplicationsByUser;
using HireFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize]
public class JobApplicationsController : ControllerBase
{
	private readonly IMediator _mediator;

	public JobApplicationsController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpPost("jobs/{jobId}")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<JobApplicationDto>> Submit(long jobId, SubmitApplicationRequest request)
	{
		var result = await _mediator.Send(new SubmitApplicationCommand(jobId, request));
		return Ok(result);
	}

	[HttpPost("{id}/status")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<JobApplicationDto>> ChangeStatus(long id, [FromQuery] ApplicationStatus newStatus)
	{
		var result = await _mediator.Send(new ChangeApplicationStatusCommand(id, newStatus));
		return Ok(result);
	}

	[HttpGet("jobs/{jobId}")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<PagedResult<JobApplicationDto>>> GetByJob(
		long jobId,
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var result = await _mediator.Send(new GetJobApplicationsByJobQuery(jobId, pageNumber, pageSize));
		return Ok(result);
	}

	[HttpGet("my")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<PagedResult<JobApplicationDto>>> GetByUser(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var result = await _mediator.Send(new GetJobApplicationsByUserQuery(pageNumber, pageSize));
		return Ok(result);
	}

	[HttpGet("{id}")]
	[Authorize]
	public async Task<ActionResult<JobApplicationDto>> GetById(long id)
	{
		var result = await _mediator.Send(new GetJobApplicationByIdQuery(id));
		if (result is null)
			return NotFound();

		return Ok(result);
	}

	// PATCH api/applications/5/withdraw
	[HttpPatch("{id}/withdraw")]
	[Authorize(Roles = "Freelancer")]
	public async Task<IActionResult> Withdraw(long id)
	{
		await _mediator.Send(new WithdrawApplicationCommand(id));
		return NoContent();
	}
}
