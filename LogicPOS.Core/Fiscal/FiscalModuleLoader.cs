using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Loads the first fiscal plugin, the same way the legacy host loaded the first software-vendor plugin.
/// Looks in plugins\*Plugin.dll, then LogicPOS.Fiscal.dll beside the app. No project reference.
/// </summary>
public static class FiscalModuleLoader
{
    public static bool TryRegister(IServiceCollection services, string baseDirectory)
    {
        foreach (var path in CandidatePaths(baseDirectory))
        {
            if (TryLoad(services, path))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<string> CandidatePaths(string baseDirectory)
    {
        var plugins = Path.Combine(baseDirectory, "plugins");
        if (Directory.Exists(plugins))
        {
            foreach (var path in Directory.GetFiles(plugins, "*Plugin.dll").OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
            {
                yield return path;
            }
        }

        var beside = Path.Combine(baseDirectory, "LogicPOS.Fiscal.dll");
        if (File.Exists(beside))
        {
            yield return beside;
        }
    }

    private static bool TryLoad(IServiceCollection services, string path)
    {
        Assembly assembly;
        try
        {
            assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        }
        catch (Exception)
        {
            return false;
        }

        foreach (var type in SafeTypes(assembly))
        {
            var method = type.GetMethod("AddFiscal", BindingFlags.Public | BindingFlags.Static);
            if (method is null)
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(IServiceCollection))
            {
                continue;
            }

            method.Invoke(null, [services]);
            return true;
        }

        foreach (var type in SafeTypes(assembly))
        {
            if (type.IsAbstract || type.IsInterface || typeof(IFiscalModule).IsAssignableFrom(type) == false)
            {
                continue;
            }

            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                continue;
            }

            services.AddSingleton(typeof(IFiscalModule), type);
            return true;
        }

        return false;
    }

    private static IEnumerable<Type> SafeTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
    }
}
