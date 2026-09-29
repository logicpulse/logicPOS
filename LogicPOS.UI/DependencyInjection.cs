using LogicPOS.Api;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Windows;

namespace LogicPOS.UI
{
    public class DependencyInjection
    {
        public static IServiceProvider Services { get; private set; }
        public static ISender Mediator { get; private set; }

        public static bool Initialize()
        {
            try
            {
                var services = new ServiceCollection();
                services.AddSerilog(Log.Logger);
                services.AddApi();
                Services = services.BuildServiceProvider();
                Mediator = Services.GetRequiredService<ISender>();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize DependencyInjection");
                System.Windows.MessageBox.Show("An error occurred while initializing the application. Please check the logs for more details.\n\n"
                                               + ex.Message,
                                               "Initialization Error",
                                               MessageBoxButton.OK,
                                               MessageBoxImage.Error);
                return false;
            }
        }
    }
}
