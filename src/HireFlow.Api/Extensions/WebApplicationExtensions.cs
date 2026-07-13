using HireFlow.Api.Middlewares;

namespace HireFlow.Api.Extensions;

public static class WebApplicationExtensions
{
	public static WebApplication UseErrorHandler(this WebApplication app)
	{
		app.UseMiddleware<ErrorHandlerMiddleware>();
		return app;
	}
}
