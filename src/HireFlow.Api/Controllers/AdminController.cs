using HireFlow.Application.DTOs.Common;
using HireFlow.Application.Features.Admin.Commands.ApproveCompany;
using HireFlow.Application.Features.Admin.Commands.DeleteUser;
using HireFlow.Application.Features.Admin.Commands.SuspendCompany;
using HireFlow.Application.Features.Admin.Dtos;
using HireFlow.Application.Features.Admin.Queries.GetCompany;
using HireFlow.Application.Features.Admin.Queries.GetUsers;
using HireFlow.Application.Features.Job.Commands.DeleteJob;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")] 
public class AdminController : ControllerBase
{
	private readonly IMediator _mediator;

	public AdminController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("users")]
	public async Task<ActionResult<PagedResult<UserSummaryDto>>> GetAllUsers(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var query = new GetUsersQuery(pageNumber, pageSize);
		var result = await _mediator.Send(query);
		return Ok(result);
	}

	[HttpGet("companies")]
	public async Task<ActionResult<PagedResult<CompanySummaryDto>>> GetAllCompanies(
		[FromQuery] int pageNumber = 1,
		[FromQuery] int pageSize = 10)
	{
		var query = new GetCompaniesQuery(pageNumber, pageSize);
		var result = await _mediator.Send(query);
		return Ok(result);
	}

	[HttpPost("{id}/approve")]
	public async Task<IActionResult> Approve(long id)
	{
		await _mediator.Send(new ApproveCompanyCommand(id));
		return NoContent(); 
	}

	[HttpPost("{id}/suspend")]
	public async Task<IActionResult> Suspend(long id)
	{
		await _mediator.Send(new SuspendCompanyCommand(id));
		return NoContent();
	}

	[HttpDelete("users/{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteUser(long id)
	{
		await _mediator.Send(new AdminDeleteUserCommand(id));
		return NoContent();
	}

	[HttpDelete("jobs/{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteJob(long id)
	{
		await _mediator.Send(new AdminDeleteJobCommand(id));
		return NoContent();
	}
}
