using FluentValidation;
using HireFlow.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HireFlow.Application.Extensions;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var assembly = Assembly.GetExecutingAssembly();

		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(assembly);

			cfg.AddBehavior(typeof(IPipelineBehavior<,>),
							typeof(ValidationBehavior<,>));
		});

		services.AddValidatorsFromAssembly(assembly);

		return services;
	}
}
