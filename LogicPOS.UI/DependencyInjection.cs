using LogicPOS.Api;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LogicPOS.UI
{
    public class DependencyInjection
    {
        public readonly static IServiceProvider Services;

        public static ISender Mediator => Services.GetRequiredService<IMediator>();

        static DependencyInjection()
        {
            var services = new ServiceCollection();

            services.AddApi();

            Services = services.BuildServiceProvider();
        }
    }
}
