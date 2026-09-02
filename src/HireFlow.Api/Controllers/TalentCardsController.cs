using HireFlow.Application.DTOs.Common;
using HireFlow.Application.DTOs.TalentCard;
using HireFlow.Application.Features.TalentCard.Commands.CreateTalentCard;
using HireFlow.Application.Features.TalentCard.Commands.DeleteTalentCard;
using HireFlow.Application.Features.TalentCard.Commands.UpdateTalentCard;
using HireFlow.Application.Features.TalentCard.Queries.GetMyTalentCards;
using HireFlow.Application.Features.TalentCard.Queries.GetTalentCardById;
using HireFlow.Application.Features.TalentCard.Queries.SearchTalentCards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/talent-cards")]
public class TalentCardsController : ControllerBase
{
	private readonly IMediator _mediator;

	public TalentCardsController(IMediator mediator) => _mediator = mediator;

	[HttpGet]
	public async Task<ActionResult<PagedResult<TalentCardDto>>> Search(
		[FromQuery] TalentCardFilterRequest filter)
	{
		var result = await _mediator.Send(new SearchTalentCardsQuery(filter));
		return Ok(result);
	}

	[HttpGet("my")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<List<TalentCardDto>>> GetMy()
	{
		var result = await _mediator.Send(new GetMyTalentCardsQuery());
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<TalentCardDto>> GetById(long id)
	{
		var result = await _mediator.Send(new GetTalentCardByIdQuery(id));
		return Ok(result);
	}

	[HttpPost]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<TalentCardDto>> Create(
		CreateTalentCardRequest request)
	{
		var result = await _mediator.Send(new CreateTalentCardCommand(request));
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id}")]
	[Authorize(Roles = "Freelancer")]
	public async Task<ActionResult<TalentCardDto>> Update(
		long id, UpdateTalentCardRequest request)
	{
		var result = await _mediator.Send(new UpdateTalentCardCommand(id, request));
		return Ok(result);
	}

	[HttpDelete("{id}")]
	[Authorize(Roles = "Freelancer")]
	public async Task<IActionResult> Delete(long id)
	{
		await _mediator.Send(new DeleteTalentCardCommand(id));
		return NoContent();
	}
}
