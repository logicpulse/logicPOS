using LogicPOS.Api;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.UI
{
    public class DependencyInjection
    {
        public readonly static IServiceProvider Services;

        static DependencyInjection()
        {
            var services = new ServiceCollection();

            services.AddHttpClient("Default", config =>
            {
                config.DefaultRequestHeaders.Add("Accept", "application/json");
                config.BaseAddress = new Uri("http://localhost:5011/");
            });

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<ApiAssemblyReference>();
            });

            Services = services.BuildServiceProvider();
        }
    }
}
