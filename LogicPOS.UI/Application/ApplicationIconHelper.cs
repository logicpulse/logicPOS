using Gtk;
using logicpos;
using LogicPOS.UI.Settings;
using System;
using System.IO;
using System.Reflection;

namespace LogicPOS.UI.Application
{
    internal static class ApplicationIconHelper
    {
        internal static void ApplyGtkDefaultIcon()
        {
            var pixbuf = TryLoadPixbuf();
            if (pixbuf == null)
                return;

            Gtk.Window.DefaultIcon = pixbuf;
        }

        internal static Gdk.Pixbuf TryLoadPixbuf()
        {
            foreach (var path in GetCandidateIconPaths())
            {
                if (!File.Exists(path))
                    continue;

                try
                {
                    using (var image = System.Drawing.Image.FromFile(path))
                    {
                        return Utils.ImageToPixbuf(image);
                    }
                }
                catch
                {
                    // Try next candidate.
                }
            }

            try
            {
                var exePath = Assembly.GetExecutingAssembly().Location;
                if (string.IsNullOrWhiteSpace(exePath))
                    return null;

                using (var icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath))
                {
                    if (icon == null)
                        return null;

                    using (var bitmap = icon.ToBitmap())
                    {
                        return Utils.ImageToPixbuf(bitmap);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static string[] GetCandidateIconPaths()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
            return new[]
            {
                Path.Combine(baseDir, AppSettings.AppIcon),
                Path.Combine(baseDir, AppSettings.Paths.Images, AppSettings.AppIcon),
                Path.Combine(baseDir, "Assets", "Images", AppSettings.AppIcon),
            };
        }
    }
}
