using HireFlow.Application.DTOs.Chat.Requests;
using HireFlow.Application.DTOs.Chat.Responses;
using HireFlow.Application.Features.Chat.Commands.MarkMessage;
using HireFlow.Application.Features.Chat.Commands.SendMessage;
using HireFlow.Application.Features.Chat.Queries.GetConversation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Controllers;

[ApiController]
[Route("api/applications/{applicationId}/messages")]
[Authorize]
public class ChatController : ControllerBase
{
	private readonly IMediator _mediator;

	public ChatController(IMediator mediator) => _mediator = mediator;

	// GET api/applications/5/messages
	[HttpGet]
	public async Task<ActionResult<ConversationDto>> GetConversation(long applicationId)
	{
		var result = await _mediator.Send(new GetConversationQuery(applicationId));
		return Ok(result);
	}

	// POST api/applications/5/messages
	[HttpPost]
	public async Task<ActionResult<MessageDto>> Send(
		long applicationId, SendMessageRequest request)
	{
		var result = await _mediator.Send(
			new SendMessageCommand(applicationId, request));
		return Ok(result);
	}

	// PATCH api/applications/5/messages/read
	[HttpPatch("read")]
	public async Task<IActionResult> MarkAsRead(long applicationId)
	{
		await _mediator.Send(new MarkMessagesAsReadCommand(applicationId));
		return NoContent();
	}
}