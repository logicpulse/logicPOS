using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;

namespace LogicPOS.Core.Licensing;

/// <summary>
/// Loads LogicPOS.Licence.dll when it sits beside the app or at LogicPOS:License:AssemblyPath.
/// The public host does not reference that project.
/// </summary>
public static class LicenseModuleLoader
{
    public static ILicenseModule Load(IConfiguration configuration, string baseDirectory, out string? startupError)
    {
        startupError = null;
        var path = ResolvePath(configuration, baseDirectory);
        if (path is null)
        {
            return new NullLicenseModule();
        }

        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        UsePrivateHardwareId(assembly);

        var gate = assembly.GetType("LogicPOS.Licence.LicenseComposition");
        var allows = gate?.GetMethod("AllowsStartup", BindingFlags.Public | BindingFlags.Static);
        var allowed = allows?.Invoke(null, [baseDirectory]) as bool? == true;
        if (allowed == false)
        {
            startupError = "É necessário registar a licença antes de usar a aplicação.";
        }

        return new RequiredLicenseModule();
    }

    private static void UsePrivateHardwareId(Assembly assembly)
    {
        var provider = assembly.GetType("LogicPOS.Licence.HardwareIdProvider");
        var method = provider?.GetMethod("GetHardwareId", BindingFlags.Public | BindingFlags.Static);
        if (method is null)
        {
            return;
        }

        MachineIdentity.UseHardwareId(() => (string)method.Invoke(null, [MachineIdentity.AppName])!);
    }

    private static string? ResolvePath(IConfiguration configuration, string baseDirectory)
    {
        var configured = configuration["LogicPOS:License:AssemblyPath"];
        if (string.IsNullOrWhiteSpace(configured) == false)
        {
            var full = Path.GetFullPath(configured);
            if (File.Exists(full))
            {
                return full;
            }

            throw new FileNotFoundException("Não encontrei LogicPOS.Licence.dll em LogicPOS:License:AssemblyPath.", full);
        }

        var beside = Path.Combine(baseDirectory, "LogicPOS.Licence.dll");
        return File.Exists(beside) ? beside : null;
    }

    private sealed class RequiredLicenseModule : ILicenseModule
    {
        public bool RegistrationRequired => true;
    }
}
