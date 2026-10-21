using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Avalonia.Media;
using LogicPOS.Core;

namespace LogicPOS.Android;

/// <summary>
/// Avalonia 12 Android: Application is generic; MainActivity is not.
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
        var dataDir = AndroidData.EnsureDataDirectory(this);
        LogicPOS.App.App.Bootstrap = () => AppComposition.Configure(dataDir);
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
