using LogicPOS.Api;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace LogicPOS.UI
{
    public class DependencyInjection
    {
        public readonly static IServiceProvider Services;
        public static ISender Mediator { get; private set; }

        static DependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddSerilog(Log.Logger);
            services.AddApi();
            Services = services.BuildServiceProvider();
            Mediator = Services.GetRequiredService<ISender>();
        }
    }
}
