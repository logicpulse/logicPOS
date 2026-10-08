using System.Text;
using Avalonia;
using Avalonia.Media;
using LogicPOS.Core;

namespace LogicPOS.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // CP860 (Portuguese DOS) is required by ESC/POS thermal tickets and the customer display.
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        AppComposition.Configure();
        if (args.Contains("--smoke"))
        {
            var report = LogicPOS.App.BackOfficeSmoke.RunAsync().GetAwaiter().GetResult();
            var logPath = Path.Combine(AppContext.BaseDirectory, "smoke-crud.log");
            File.WriteAllText(logPath, report, new System.Text.UTF8Encoding(true));
            Environment.Exit(report.StartsWith("SMOKE OK", StringComparison.Ordinal) ? 0 : 1);
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<LogicPOS.App.App>()
            .UsePlatformDetect()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://LogicPOS.App/Assets/Fonts#Montserrat"
            })
            .LogToTrace();
}