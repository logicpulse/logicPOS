using Avalonia.Media;

namespace LogicPOS.App.Themes;

/// <summary>
/// Applies the tunable button gloss level to Glass.Button.* resources
/// defined in Themes/GlassButton.axaml. Level 50 matches the design tokens.
/// </summary>
public static class GlassButtonTheme
{
    public const string PreferenceToken = "APP_BUTTON_GLOSS";
    public const int DefaultLevel = 50;
    public const string SheenKey = "Glass.Button.Sheen";
    public const string ShadowKey = "Glass.Button.Shadow";

    public static int ParseLevel(string? value)
    {
        if (int.TryParse(value?.Trim(), out var level))
        {
            return Math.Clamp(level, 0, 100);
        }

        return DefaultLevel;
    }

    public static void Apply(int level)
    {
        level = Math.Clamp(level, 0, 100);
        var app = global::Avalonia.Application.Current;
        if (app is null)
        {
            return;
        }

        // Design baseline (level 50): top #8CFFFFFF, mid #1FFFFFFF, shadow #1A000000.
        var scale = level / (double)DefaultLevel;
        var top = (byte)Math.Clamp(Math.Round(0x8C * scale), 0, 255);
        var mid = (byte)Math.Clamp(Math.Round(0x1F * scale), 0, 255);
        var shadowAlpha = (byte)Math.Clamp(Math.Round(0x1A * scale), 0, 255);

        var sheen = new LinearGradientBrush
        {
            StartPoint = new global::Avalonia.RelativePoint(0, 0, global::Avalonia.RelativeUnit.Relative),
            EndPoint = new global::Avalonia.RelativePoint(0, 1, global::Avalonia.RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(top, 255, 255, 255), 0),
                new GradientStop(Color.FromArgb(mid, 255, 255, 255), 0.4),
                new GradientStop(Color.FromArgb(0, 255, 255, 255), 1),
            },
        };

        var shadow = level <= 0
            ? default
            : BoxShadows.Parse($"0 2 6 0 #{shadowAlpha:X2}000000");

        app.Resources[SheenKey] = sheen;
        app.Resources[ShadowKey] = shadow;
    }

    public static void ApplyFromPreference(string? value) => Apply(ParseLevel(value));
}