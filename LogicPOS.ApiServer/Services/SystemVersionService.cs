using System.Diagnostics;
using System.Reflection;

namespace LogicPOS.ApiServer.Services;

public sealed class SystemVersionService
{
    public string GetApiVersion()
    {
        var assembly = typeof(Program).Assembly;
        var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        if (TryParseVersion(informational, out var informationalVersion))
        {
            return informationalVersion.ToString(3);
        }

        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        if (TryParseVersion(fileVersionInfo.FileVersion, out var fileVersion))
        {
            return fileVersion.ToString(3);
        }

        var assemblyVersion = assembly.GetName().Version;
        if (assemblyVersion is not null)
        {
            return new Version(assemblyVersion.Major, assemblyVersion.Minor, Math.Max(assemblyVersion.Build, 0)).ToString(3);
        }

        return "0.0.0";
    }

    private static bool TryParseVersion(string? candidate, out Version version)
    {
        version = new Version(0, 0, 0);

        if (string.IsNullOrWhiteSpace(candidate))
        {
            return false;
        }

        var plus = candidate.IndexOf('+');
        if (plus > 0)
        {
            candidate = candidate.Substring(0, plus);
        }

        var dash = candidate.IndexOf('-');
        if (dash > 0)
        {
            candidate = candidate.Substring(0, dash);
        }

        if (!Version.TryParse(candidate.Trim(), out var parsed))
        {
            return false;
        }

        version = new Version(parsed.Major, parsed.Minor, Math.Max(parsed.Build, 0));
        return true;
    }
}
