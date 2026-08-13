using HireFlow.Api.Common.File;
using HireFlow.Application.DTOs.User;
using HireFlow.Application.Features.Common.User.Commands.UpdateCompanyProfile;
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

	// POST api/profile/freelancer/cv
	[HttpPost("freelancer/cv")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<string>> UploadCv(IFormFile file)
	{
		FileValidator.ValidateCv(file);

		var url = await _fileStorageService.SaveAsync(
			file.OpenReadStream(),
			file.FileName,
			folder: "cvs");

		// Save URL to user profile
		await _mediator.Send(new UpdateFreelancerProfileCommand(
			new UpdateFreelancerProfileRequest { CvUrl = url }));

		return Ok(new { url });
	}

	// POST api/profile/freelancer/avatar
	[HttpPost("freelancer/avatar")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<string>> UploadAvatar(IFormFile file)
	{
		FileValidator.ValidateImage(file);

		var url = await _fileStorageService.SaveAsync(
			file.OpenReadStream(),
			file.FileName,
			folder: "avatars");

		await _mediator.Send(new UpdateFreelancerProfileCommand(
			new UpdateFreelancerProfileRequest { ProfilePictureUrl = url }));

		return Ok(new { url });
	}

	// POST api/profile/company/logo
	[HttpPost("company/logo")]
	[Authorize(Roles = "Company")]
	public async Task<ActionResult<string>> UploadLogo(IFormFile file)
	{
		FileValidator.ValidateImage(file);

		var url = await _fileStorageService.SaveAsync(
			file.OpenReadStream(),
			file.FileName,
			folder: "logos");

		await _mediator.Send(new UpdateCompanyProfileCommand(
			new UpdateCompanyProfileRequest { LogoUrl = url }));

		return Ok(new { url });
	}
}