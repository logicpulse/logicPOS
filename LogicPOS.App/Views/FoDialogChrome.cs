using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace LogicPOS.App.Views;

/// <summary>
/// FO dialog shells already show the page title in the dark titlebar.
/// Hide the inner BO page-title row in code — Avalonia type selectors like
/// <c>UserControl.fo_dialog_host</c> do not match concrete hosts (StyleKey is the derived type).
/// </summary>
internal static class FoDialogChrome
{
    public static void ApplyHosted(Control? host)
    {
        if (host is null)
        {
            return;
        }

        host.Classes.Set("fo_dialog_host", true);
        HidePageTitleRow(host);
    }

    public static void HidePageTitleRow(Control host)
    {
        if (host.FindControl<Control>("PageTitleRow") is { } named)
        {
            named.IsVisible = false;
            return;
        }

        // Host may not be fully attached yet; walk visual children when possible.
        foreach (var panel in host.GetVisualDescendants().OfType<StackPanel>())
        {
            if (panel.Name == "PageTitleRow" || panel.Classes.Contains("bo_page_title_row"))
            {
                panel.IsVisible = false;
                return;
            }
        }
    }
}