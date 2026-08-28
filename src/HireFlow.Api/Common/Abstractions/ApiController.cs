using HireFlow.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Common.Abstractions;

[ApiController]
public abstract class ApiController : ControllerBase
{
	protected readonly ISender Sender;

	protected ApiController(ISender sender) => Sender = sender;

	protected IActionResult HandleFailure(Result result)
	{
		if (result.IsSuccess)
			throw new InvalidOperationException("HandleFailure can only be called with a failed result.");

		return BadRequest(new ProblemDetails
		{
			Title = "Bad Request",
			Type = result.Error.Code,
			Detail = result.Error.Message,
			Status = StatusCodes.Status400BadRequest
		});
	}
}
