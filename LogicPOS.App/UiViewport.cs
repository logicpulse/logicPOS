using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace LogicPOS.App;

/// <summary>
/// Screen size used for layout: prefers the window client area, falls back to the
/// working area of the current screen. Handles landscape and portrait.
/// </summary>
public readonly record struct UiViewport(int Width, int Height)
{
    public const int DesignLandscapeWidth = 1920;
    public const int DesignLandscapeHeight = 1080;
    public const int DesignPortraitWidth = 1080;
    public const int DesignPortraitHeight = 1920;

    public bool IsPortrait => Height > Width;
    public bool IsLandscape => Width >= Height;
    public double AspectRatio => Height <= 0 ? 0 : (double)Width / Height;

    public int DesignWidth => IsPortrait ? DesignPortraitWidth : DesignLandscapeWidth;
    public int DesignHeight => IsPortrait ? DesignPortraitHeight : DesignLandscapeHeight;

    public static UiViewport From(Window window)
    {
        var client = window.ClientSize;
        if (client.Width >= 320 && client.Height >= 320)
        {
            return new UiViewport(
                Math.Max(1, (int)Math.Round(client.Width)),
                Math.Max(1, (int)Math.Round(client.Height)));
        }

        var screen = window.Screens?.ScreenFromVisual(window) ?? window.Screens?.Primary;
        return From(screen);
    }

    public static UiViewport From(Screen? screen)
    {
        if (screen is null)
        {
            return new UiViewport(DesignLandscapeWidth, DesignLandscapeHeight);
        }

        // Avalonia Screen bounds are already in the same units as Window.ClientSize / layout.
        var area = screen.WorkingArea;
        var width = Math.Max(1, (int)Math.Round((double)area.Width));
        var height = Math.Max(1, (int)Math.Round((double)area.Height));
        if (width < 320 || height < 320)
        {
            width = Math.Max(1, (int)Math.Round((double)screen.Bounds.Width));
            height = Math.Max(1, (int)Math.Round((double)screen.Bounds.Height));
        }

        return new UiViewport(width, height);
    }
}
