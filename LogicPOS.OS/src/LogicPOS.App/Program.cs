using Avalonia;
using Avalonia.Media;
using LogicPOS.Core;

namespace LogicPOS.App;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppComposition.Configure();
        if (args.Contains("--smoke"))
        {
            var report = BackOfficeSmoke.RunAsync().GetAwaiter().GetResult();
            var logPath = Path.Combine(AppContext.BaseDirectory, "smoke-crud.log");
            File.WriteAllText(logPath, report, new System.Text.UTF8Encoding(true));
            Environment.Exit(report.StartsWith("SMOKE OK", StringComparison.Ordinal) ? 0 : 1);
        }

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://logicpos/Assets/Fonts#Montserrat"
            })
            .LogToTrace();
}
