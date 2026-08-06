using LogicPOS.UI.Settings;
using Serilog;
using System;
using System.Drawing;
using System.IO;

namespace LogicPOS.UI.Components.Licensing.Branding
{
    /// <summary>
    /// Resolves branding logos from license reseller and decodes them via the
    /// dynamically loaded LogicPOS.Plugins.Branding plugin.
    /// Always returns images at native pixel size (no stretch) — matches legacy Login/FO.
    /// </summary>
    public static class BrandingImageService
    {
        // Legacy FO and Login both use the *_login.png asset (not *_fo.png).
        private const string DefaultBrandingFileName = "logicPOS_logicpulse_login.png";
        private const string DisplayFallbackFileName = "logicPOS_logo.png";
        private const string BrandingFolderName = "Branding";

        /// <summary>
        /// Loads the branding plugin. Call after license data has been loaded.
        /// </summary>
        public static void Initialize()
        {
            BrandingPluginLoader.Initialize();
        }

        /// <summary>
        /// Decodes the branding logo (or loads the display fallback) at native pixel size.
        /// </summary>
        public static Bitmap CreateBitmap(BrandingLogoKind kind)
        {
            var logoPath = ResolveLogoPath(kind);
            if (!string.IsNullOrWhiteSpace(logoPath) && BrandingPluginLoader.IsAvailable)
            {
                try
                {
                    // Plugin API accepts width/height but BrandingItens ignores them (native decode).
                    var decoded = BrandingPluginLoader.DecodeImage(logoPath, 0, 0);
                    if (decoded != null)
                    {
                        Log.Debug("Branding logo decoded from {Path} ({Width}x{Height})", logoPath, decoded.Width, decoded.Height);
                        return new Bitmap(decoded);
                    }

                    Log.Warning("Branding decode returned null for {Path}; using default logo", logoPath);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Branding decode failed for {Path}; using default logo", logoPath);
                }
            }

            return CreateDisplayFallbackBitmap();
        }

        private static string ResolveLogoPath(BrandingLogoKind kind)
        {
            // kind kept for callers; both Login and FrontOffice use the same legacy login asset.
            _ = kind;
            var fileName = DefaultBrandingFileName;

            var imagesDirectory = AppSettings.Paths.GetThemeFileLocation("Images");
            var resellerFolder = ResolveResellerFolder(LicensingService.Data?.Reseller);

            Log.Information("Resolving branding logo for kind {Kind} and reseller folder {ResellerFolder}", kind, resellerFolder);

            if (!string.IsNullOrWhiteSpace(resellerFolder))
            {
                var resellerPath = Path.Combine(imagesDirectory, BrandingFolderName, resellerFolder, fileName);
                if (File.Exists(resellerPath))
                {
                    return resellerPath;
                }

                Log.Warning(
                    "Branding logo for reseller folder {ResellerFolder} not found at {Path}; trying LogicPulse default branding file",
                    resellerFolder,
                    resellerPath);
            }

            var defaultPath = Path.Combine(imagesDirectory, fileName);
            if (File.Exists(defaultPath))
            {
                return defaultPath;
            }

            Log.Warning("Default branding logo file not found at {Path}", defaultPath);
            return null;
        }

        /// <summary>
        /// Maps license reseller name to Branding subfolder (legacy product-key → license.reseller flow).
        /// Returns null for LogicPulse / empty / unknown (use default image).
        /// </summary>
        internal static string ResolveResellerFolder(string reseller)
        {
            if (string.IsNullOrWhiteSpace(reseller))
            {
                return null;
            }

            if (string.Equals(reseller, "Logicpulse", StringComparison.OrdinalIgnoreCase)
                || string.Equals(reseller, "LogicPulse", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            switch (reseller)
            {
                case "NewTech":
                    return "NT";
                case "SwissConsultings GmbH":
                    return "SW";
                case "Informurça":
                    return "IM";
                default:
                    return null;
            }
        }

        private static Bitmap CreateDisplayFallbackBitmap()
        {
            var fallbackPath = Path.Combine(AppSettings.Paths.GetThemeFileLocation("Images"), DisplayFallbackFileName);
            if (!File.Exists(fallbackPath))
            {
                Log.Error("Display fallback logo not found at {Path}", fallbackPath);
                return new Bitmap(1, 1);
            }

            return new Bitmap(fallbackPath);
        }
    }
}
