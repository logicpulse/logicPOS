using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Loads LogicPOS.Fiscal.dll when it sits beside the app or under logicpos-fiscal.
/// The public host does not reference that project.
/// </summary>
public static class FiscalModuleLoader
{
    public static bool TryRegister(IServiceCollection services)
    {
        var path = ResolvePath();
        if (path is null)
        {
            return false;
        }

        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        var type = assembly.GetType("LogicPOS.Fiscal.FiscalComposition");
        var method = type?.GetMethod("AddFiscal", BindingFlags.Public | BindingFlags.Static);
        if (method is null)
        {
            return false;
        }

        method.Invoke(null, [services]);
        return true;
    }

    private static string? ResolvePath()
    {
        var beside = Path.Combine(AppContext.BaseDirectory, "LogicPOS.Fiscal.dll");
        if (File.Exists(beside))
        {
            return beside;
        }

        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            foreach (var build in new[] { "Debug", "Release" })
            {
                var candidate = Path.Combine(
                    directory.FullName,
                    "logicpos-fiscal",
                    "src",
                    "LogicPOS.Fiscal",
                    "bin",
                    build,
                    "net10.0",
                    "LogicPOS.Fiscal.dll");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            directory = directory.Parent;
        }

        return null;
    }
}
