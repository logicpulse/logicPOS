using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Services;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LogicPOS.UI.Application.Services
{
    public static class SystemVersionService
    {
        public static Version PosVersion { get; private set; }
        public static Version ApiVersion { get; private set; }
        public static Version LastestVersion { get; private set; }
        public static Version LatestVersionFromLicense { get; private set; }

        /// <summary>retail | develop (from InformationalVersion metadata).</summary>
        public static string PosChannel { get; private set; } = "retail";

        /// <summary>e.g. "1.5.2 retail"</summary>
        public static string PosDisplayVersion { get; private set; } = "0.0.0";

        public static void Initialize()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                                ?? fileVersionInfo.ProductVersion
                                ?? fileVersionInfo.FileVersion
                                ?? "0.0.0";

            PosChannel = ParseChannel(informational);
            PosVersion = ParseNumericVersion(informational, fileVersionInfo);
            PosDisplayVersion = $"{PosVersion.ToString(3)} {PosChannel}";

            LastestVersion = LicensingService.GetLatestSystemVersion();
            LatestVersionFromLicense = LicensingService.GetLatestVersionFromLicense();
            ApiVersion = SystemInformationService.GetApiVersion();
            Log.Information("LogicPOS version: {DisplayVersion} (numeric {Version})", PosDisplayVersion, PosVersion);
            Log.Information("API version: {Version}", SystemVersionService.ApiVersion);
        }

        private static string ParseChannel(string informational)
        {
            if (string.IsNullOrWhiteSpace(informational))
                return "retail";

            var plus = informational.LastIndexOf('+');
            if (plus < 0 || plus >= informational.Length - 1)
                return "retail";

            var channel = informational.Substring(plus + 1).Trim().ToLowerInvariant();
            // Strip any extra build metadata after channel (rare)
            channel = channel.Split(new[] { '.', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? channel;
            if (channel == "develop" || channel == "dev")
                return "develop";
            if (channel == "retail" || channel == "stable")
                return "retail";
            return channel;
        }

        private static Version ParseNumericVersion(string informational, FileVersionInfo fileVersionInfo)
        {
            var candidate = informational;
            var plus = candidate.IndexOf('+');
            if (plus > 0)
                candidate = candidate.Substring(0, plus);
            var dash = candidate.IndexOf('-');
            if (dash > 0)
                candidate = candidate.Substring(0, dash);

            Version fromInfo;
            if (Version.TryParse(candidate.Trim(), out fromInfo))
                return new Version(fromInfo.Major, fromInfo.Minor, Math.Max(fromInfo.Build, 0));

            Version fromFile;
            if (!string.IsNullOrWhiteSpace(fileVersionInfo.FileVersion) &&
                Version.TryParse(fileVersionInfo.FileVersion, out fromFile))
                return new Version(fromFile.Major, fromFile.Minor, Math.Max(fromFile.Build, 0));

            return new Version(0, 0, 0);
        }
    }
}
