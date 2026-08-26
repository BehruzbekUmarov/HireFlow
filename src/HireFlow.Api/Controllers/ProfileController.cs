using HireFlow.Api.Common.File;
using HireFlow.Application.DTOs.User.Requests;
using HireFlow.Application.DTOs.User.Responses;
using HireFlow.Application.Features.Common.User.Commands.UpdateCompanyLogo;
using HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;
using HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerAvatar;
using HireFlow.Application.Features.Common.User.Commands.UpdateFreelancerProfile;
using HireFlow.Application.Features.Common.User.Queries.GetCompanyProfile;
using HireFlow.Application.Features.Common.User.Queries.GetFreelancerProfile;
using HireFlow.Application.Services.Interfaces;
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
	private readonly IFileStorageService _fileStorageService;

	public ProfileController(IMediator mediator, IFileStorageService fileStorageService)
	{
		_mediator = mediator;
		_fileStorageService = fileStorageService;
	}

	[HttpGet("freelancer")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<FreelancerProfileDto>> GetFreelancerProfile()
	{
		var result = await _mediator.Send(new GetFreelancerProfileQuery());
		return Ok(result);
	}

	[HttpPut("freelancer")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<FreelancerProfileDto>> UpdateFreelancerProfile(
		UpdateFreelancerProfileRequest request)
	{
		var result = await _mediator.Send(
			new UpdateFreelancerProfileCommand(request));
		return Ok(result);
	}

	[HttpGet("company")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<CompanyProfileDto>> GetCompanyProfile()
	{
		var result = await _mediator.Send(new GetCompanyProfileQuery());
		return Ok(result);
	}

	[HttpPut("company")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<CompanyProfileDto>> UpdateCompanyProfile(
		UpdateCompanyProfileRequest request)
	{
		var result = await _mediator.Send(
			new UpdateCompanyProfileCommand(request));
		return Ok(result);
	}

	[HttpPost("freelancer/avatar")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<string>> UploadAvatar(IFormFile file)
	{
		FileValidator.ValidateImage(file);

		var url = await _fileStorageService.SaveAsync(
			file.OpenReadStream(),
			file.FileName,
			folder: "avatars");

		await _mediator.Send(new UpdateFreelancerAvatarCommand(url));

		return Ok(new { url });
	}

	[HttpPost("company/logo")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<string>> UploadLogo(IFormFile file)
	{
		FileValidator.ValidateImage(file);

		var url = await _fileStorageService.SaveAsync(
			file.OpenReadStream(),
			file.FileName,
			folder: "logos");

		await _mediator.Send(new UpdateCompanyLogoCommand(url)); 

		return Ok(new { url });
	}
}