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

            services.AddApi();

            Services = services.BuildServiceProvider();
        }
    }
}
