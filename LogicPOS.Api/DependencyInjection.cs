using LogicPOS.Api.Features.Common;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("apisettings.json");
            var configuration = configurationBuilder.Build();

            var apiSettings = configuration.Get<ApiSettings>();

            services.AddHttpClient("Default", config =>
            {
                config.DefaultRequestHeaders.Add("Accept", "application/json");
                config.BaseAddress = new Uri(apiSettings.BaseAddress);
            });

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<ApiAssemblyReference>();
               
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>));

            return services;
        }
    }
}
