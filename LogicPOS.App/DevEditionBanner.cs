using System.Reflection;
using Avalonia.Controls;
using LogicPOS.Core;
using LogicPOS.Core.Fiscal;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App;

/// <summary>
/// Visibility helper for the public open-source development strip.
/// Hidden on LogicPulse Host (certified) entry points.
/// </summary>
internal static class DevEditionBanner
{
    public const string Message =
        "Versão de desenvolvimento — não utilize para emitir documentos fiscais verdadeiros.";

    public static bool ShouldShow()
    {
        var entry = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
        if (entry.Contains("Host", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (entry.Contains("Desktop", StringComparison.OrdinalIgnoreCase)
            || entry.Contains("Android", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var fiscal = AppComposition.Services?.GetService<IFiscalModule>();
        return fiscal is null || fiscal.IsAvailable == false;
    }

    public static void Apply(Border? banner)
    {
        if (banner is null)
        {
            return;
        }

        banner.IsVisible = ShouldShow();
    }
}
