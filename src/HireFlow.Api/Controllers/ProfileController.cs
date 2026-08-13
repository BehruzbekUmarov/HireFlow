using HireFlow.Application.DTOs.User;
using HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;
using HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerProfile;
using HireFlow.Application.Features.Common.User.Queries.GetCompanyProfile;
using HireFlow.Application.Features.Common.User.Queries.GetFreelancerProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
	private readonly IMediator _mediator;

	public ProfileController(IMediator mediator)
		=> _mediator = mediator;

	// GET api/profile/freelancer
	[HttpGet("freelancer")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<FreelancerProfileDto>> GetFreelancerProfile()
	{
		var result = await _mediator.Send(new GetFreelancerProfileQuery());
		return Ok(result);
	}

	// PUT api/profile/freelancer
	[HttpPut("freelancer")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<FreelancerProfileDto>> UpdateFreelancerProfile(
		UpdateFreelancerProfileRequest request)
	{
		var result = await _mediator.Send(
			new UpdateFreelancerProfileCommand(request));
		return Ok(result);
	}

	// GET api/profile/company
	[HttpGet("company")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<CompanyProfileDto>> GetCompanyProfile()
	{
		var result = await _mediator.Send(new GetCompanyProfileQuery());
		return Ok(result);
	}

	// PUT api/profile/company
	[HttpPut("company")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<CompanyProfileDto>> UpdateCompanyProfile(
		UpdateCompanyProfileRequest request)
	{
		var result = await _mediator.Send(
			new UpdateCompanyProfileCommand(request));
		return Ok(result);
	}
}