using HireFlow.Api.Common.File;
using HireFlow.Application.DTOs.Cv.Requests;
using HireFlow.Application.DTOs.Cv.Responses;
using HireFlow.Application.Features.Cv.Commands.CreateCv;
using HireFlow.Application.Features.Cv.Commands.DeleteCv;
using HireFlow.Application.Features.Cv.Commands.SetDefaultCv;
using HireFlow.Application.Features.Cv.Commands.UpdateCv;
using HireFlow.Application.Features.Cv.Commands.UploadCvFile;
using HireFlow.Application.Features.Cv.Queries.DownloadCv;
using HireFlow.Application.Features.Cv.Queries.GetCvById;
using HireFlow.Application.Features.Cv.Queries.GetMyCvs;
using HireFlow.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/cvs")]
[Authorize(Roles = "Freelancer")]
public class CvController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly IFileStorageService _fileStorageService;

	public CvController(IMediator mediator, IFileStorageService fileStorageService)
	{
		_mediator = mediator;
		_fileStorageService = fileStorageService;
	}

	[HttpGet]
	public async Task<ActionResult<List<CvDto>>> GetMyCvs()
	{
		var result = await _mediator.Send(new GetMyCvsQuery());
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<CvDto>> GetById(long id)
	{
		var result = await _mediator.Send(new GetCvByIdQuery(id));
		return Ok(result);
	}

	[HttpPost]
	public async Task<ActionResult<CvDto>> Create(CreateCvRequest request)
	{
		var result = await _mediator.Send(new CreateCvCommand(request));
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult<CvDto>> Update(long id, UpdateCvRequest request)
	{
		var result = await _mediator.Send(new UpdateCvCommand(id, request));
		return Ok(result);
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		await _mediator.Send(new DeleteCvCommand(id));
		return NoContent();
	}

	[HttpPatch("{id}/set-default")]
	public async Task<IActionResult> SetDefault(long id)
	{
		await _mediator.Send(new SetDefaultCvCommand(id));
		return NoContent();
	}

	[HttpPost("upload")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<CvDto>> UploadCvFile(
	IFormFile file,
	[FromForm] string title = "Uploaded CV")  
	{
		FileValidator.ValidateCv(file);

		var url = await _fileStorageService.SaveAsync(
			file.OpenReadStream(),
			file.FileName,
			folder: "cvs");

		var result = await _mediator.Send(new UploadCvFileCommand(url, title));
		return Ok(result);
	}

	[HttpGet("{id}/download")]
	[Authorize(Roles = "Freelancer")]
	public async Task<IActionResult> Download(long id)
	{
		var result = await _mediator.Send(new DownloadCvQuery(id));

		// Uploaded file — redirect directly
		if (result.FileUrl is not null)
			return Redirect(result.FileUrl);

		// Generated PDF — return as file download
		return File(result.PdfBytes!, "application/pdf", result.FileName);
	}
}
