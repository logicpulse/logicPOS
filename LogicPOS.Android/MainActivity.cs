using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia.Android;
using Avalonia.Controls.ApplicationLifetimes;
using LogicPOS.App.Views;

namespace LogicPOS.Android;

[Activity(
    Label = "logicPOS",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/Icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation
        | ConfigChanges.ScreenSize
        | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout
        | ConfigChanges.SmallestScreenSize
        | ConfigChanges.Density)]
public class MainActivity : AvaloniaMainActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // Activity recreate (MIUI): Avalonia cannot reparent an existing MainView.
        if (Avalonia.Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime single
            && single.MainView is { Parent: not null })
        {
            single.MainView = new MobileHostView();
        }

        base.OnCreate(savedInstanceState);
    }
}
