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
        // Host/retail bootstrap sets App.Bootstrap — always hide the dev banner.
        if (App.Bootstrap is not null)
        {
            return false;
        }

        // Android retail (LogicPOS.Host.Android) — nunca mostrar a faixa vermelha.
        if (OperatingSystem.IsAndroid())
        {
            return false;
        }

        var entryAssembly = Assembly.GetEntryAssembly();
        var entry = entryAssembly?.GetName().Name ?? string.Empty;
        if (entry.Contains("Host", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Retail channel (e.g. 1.6.2+retail) — no public-edition strip.
        var info = entryAssembly
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? string.Empty;
        if (info.Contains("+retail", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Android: GetEntryAssembly() returns null. Scan loaded assemblies for a host marker.
        if (entryAssembly is null)
        {
            return !HasHostAssemblyLoaded();
        }

        if (entry.Contains("Desktop", StringComparison.OrdinalIgnoreCase)
            || entry.Contains("Android", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var fiscal = AppComposition.Services?.GetService<IFiscalModule>();
        return fiscal is null || fiscal.IsAvailable == false;
    }

    private static bool HasHostAssemblyLoaded()
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = asm.GetName().Name ?? string.Empty;
            if (name.Contains("Host", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var ver = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? string.Empty;
            if (ver.Contains("+retail", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
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
