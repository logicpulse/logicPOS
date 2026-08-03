using Serilog;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace LogicPOS.UI.Components.Licensing.Branding
{
    /// <summary>
    /// Loads LogicPOS.Plugins.Branding.dll dynamically from the GTK app directory.
    /// Does not reference the plugin assembly at compile time.
    /// </summary>
    internal static class BrandingPluginLoader
    {
        private const string PluginAssemblyFileName = "LogicPOS.Plugins.Branding.dll";
        private const string PluginTypeName = "LogicPOS.Plugins.Branding.Plugin";
        private const string DecodeMethodName = "DecodeImage";
        private const string BrandingItensFileName = "BrandingItens.dll";

        private static readonly object Sync = new object();
        private static object _pluginInstance;
        private static MethodInfo _decodeImageMethod;
        private static bool _initialized;
        private static bool _available;

        public static bool IsAvailable
        {
            get
            {
                EnsureInitialized();
                return _available;
            }
        }

        public static void Initialize()
        {
            EnsureInitialized();
        }

        public static Image DecodeImage(string filePath, int width, int height)
        {
            EnsureInitialized();
            if (!_available || _pluginInstance == null || _decodeImageMethod == null)
            {
                return null;
            }

            try
            {
                return _decodeImageMethod.Invoke(
                    _pluginInstance,
                    new object[] { filePath, width, height }) as Image;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Branding plugin DecodeImage failed for {Path}", filePath);
                return null;
            }
        }

        private static void EnsureInitialized()
        {
            if (_initialized)
            {
                return;
            }

            lock (Sync)
            {
                if (_initialized)
                {
                    return;
                }

                _initialized = true;
                TryLoadPlugin();
            }
        }

        private static void TryLoadPlugin()
        {
            try
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var pluginPath = Path.Combine(baseDirectory, PluginAssemblyFileName);
                var brandingItensPath = Path.Combine(baseDirectory, BrandingItensFileName);

                if (!File.Exists(pluginPath))
                {
                    Log.Warning("Branding plugin not found at {Path}; default logos will be used", pluginPath);
                    return;
                }

                if (!File.Exists(brandingItensPath))
                {
                    Log.Warning("BrandingItens.dll not found at {Path}; default logos will be used", brandingItensPath);
                    return;
                }

                var assembly = Assembly.LoadFrom(pluginPath);
                var pluginType = assembly.GetType(PluginTypeName, throwOnError: false);
                if (pluginType == null)
                {
                    Log.Error("Type {Type} not found in {Assembly}", PluginTypeName, pluginPath);
                    return;
                }

                _pluginInstance = Activator.CreateInstance(pluginType);
                _decodeImageMethod = pluginType.GetMethod(
                    DecodeMethodName,
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(string), typeof(int), typeof(int) },
                    null);

                if (_decodeImageMethod == null)
                {
                    Log.Error("Method {Method} not found on {Type}", DecodeMethodName, PluginTypeName);
                    _pluginInstance = null;
                    return;
                }

                _available = true;
                Log.Information("Branding plugin loaded from {Path}", pluginPath);
            }
            catch (Exception ex)
            {
                _available = false;
                _pluginInstance = null;
                _decodeImageMethod = null;
                Log.Error(ex, "Failed to load branding plugin; default logos will be used");
            }
        }
    }
}
