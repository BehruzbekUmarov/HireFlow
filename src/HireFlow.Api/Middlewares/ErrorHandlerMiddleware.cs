using HireFlow.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.Api.Middlewares;

internal sealed class ErrorHandlerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ErrorHandlerMiddleware> _logger;

	public ErrorHandlerMiddleware(
		RequestDelegate next,
		ILogger<ErrorHandlerMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleAsync(ex, context);
		}
	}

	private async Task HandleAsync(Exception exception, HttpContext context)
	{
		_logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

		var details = GetErrorDetails(exception);

		context.Response.StatusCode = details.Status!.Value;

		await context.Response.WriteAsJsonAsync(details);
	}

	private static ProblemDetails GetErrorDetails(Exception exception)
		=> exception switch
		{
			CompanyNotApprovedException => new ProblemDetails
			{
				Status = StatusCodes.Status403Forbidden,
				Title = "Company not approved",
				Detail = exception.Message
			},
			ConflictException => new ProblemDetails
			{
				Status = StatusCodes.Status409Conflict,
				Title = "Conflict",
				Detail = exception.Message
			},
			DuplicateApplicationException => new ProblemDetails
			{
				Status = StatusCodes.Status409Conflict,
				Title = "Duplicate application",
				Detail = exception.Message
			},
			ForbiddenException => new ProblemDetails
			{
				Status = StatusCodes.Status403Forbidden,
				Title = "Forbidden",
				Detail = exception.Message
			},
			InvalidOperationDomainException => new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Invalid operation",
				Detail = exception.Message
			},
			InvalidStatusTransitionException => new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Invalid status transition",
				Detail = exception.Message
			},
			NotFoundException => new ProblemDetails
			{
				Status = StatusCodes.Status404NotFound,
				Title = "Not found",
				Detail = exception.Message
			},
			_ => new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Internal Server Error",
				Detail = exception.Message
			}
		};
}
