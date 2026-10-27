using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Avalonia.Android;
using Avalonia.Controls.ApplicationLifetimes;
using LogicPOS.App;
using LogicPOS.App.Views;

namespace LogicPOS.Android;

[Activity(
    Label = "logicPOS",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/Icon",
    MainLauncher = true,
    WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustNothing,
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

        // Registar callback para que os ecrãs Avalonia consigam fechar a app no Android.
        LogicPOS.App.App.ExitApplication ??= () =>
        {
            FinishAffinity();
            Java.Lang.JavaSystem.Exit(0);
        };

        base.OnCreate(savedInstanceState);
        HideSoftInput();
    }

    protected override void OnResume()
    {
        base.OnResume();
        HideSoftInput();
    }

    private void HideSoftInput()
    {
        try
        {
            var imm = GetSystemService(InputMethodService) as InputMethodManager;
            var token = CurrentFocus?.WindowToken ?? Window?.DecorView?.WindowToken;
            if (imm is not null && token is not null)
            {
                imm.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
            }

            Window?.SetSoftInputMode(SoftInput.StateAlwaysHidden | SoftInput.AdjustNothing);
        }
        catch
        {
            // Ignore IME failures on exotic firmwares.
        }
    }
}
