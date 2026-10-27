using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Avalonia.Media;
using LogicPOS.Host;

namespace LogicPOS.Host.Android;

/// <summary>
/// Avalonia 12 Android retail entry (LogicPulse Host composition).
/// </summary>
[Application]
public class Application : AvaloniaAndroidApplication<LogicPOS.App.App>
{
    protected Application(nint javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    public override void OnCreate()
    {
        // Bootstrap must be set before Avalonia creates App / MobileHostView.
        var dataDir = LogicPOS.Android.AndroidData.EnsureDataDirectory(this);
        LogicPOS.App.App.Bootstrap = () => LogicPulseComposition.Configure(dataDir);
        base.OnCreate();
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://LogicPOS.App/Assets/Fonts#Montserrat"
            });
    }
}
