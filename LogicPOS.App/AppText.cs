using LogicPOS.Core.BackOffice;

namespace LogicPOS.App;

/// <summary>BackOffice sidebar captions — delegates to <see cref="SidebarMenuText"/> (app UiCulture, not OS).</summary>
internal static class AppText
{
    public static string Get(string key) => SidebarMenuText.Get(key);
}
