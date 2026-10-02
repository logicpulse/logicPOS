using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LogicPOS.App.Views;

namespace LogicPOS.App;

public partial class App : global::Avalonia.Application
{
    public override void Initialize()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            WriteCrash("UnhandledException", args.ExceptionObject as Exception);
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            WriteCrash("UnobservedTaskException", args.Exception);
            args.SetObserved();
        };

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Dispatcher.UIThread.UnhandledException += (_, args) =>
        {
            WriteCrash("UIThread.UnhandledException", args.Exception);
            args.Handled = true;
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new LoginWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    internal static void WriteCrash(string source, Exception? exception)
    {
        try
        {
            var path = Path.Combine(Path.GetTempPath(), "logicpos-login-crash.txt");
            var text = $"{DateTime.Now:O}{Environment.NewLine}{source}{Environment.NewLine}{exception}{Environment.NewLine}";
            File.AppendAllText(path, text);
            Debug.WriteLine(text);
        }
        catch
        {
            // Ignore logging failures.
        }
    }
}
