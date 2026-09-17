using Gtk;
using LogicPOS.UI.Extensions;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Dialogs
{
    internal static class DialogMaskHelper
    {
        public static List<Window> CreateAndShow(Window parent, float opacity = 0.35f)
        {
            var masks = new List<Window>();
            var screen = Gdk.Screen.Default;

            if (screen == null || screen.NMonitors <= 0)
            {
                masks.Add(CreateFullscreenMask(parent, opacity));
                return masks;
            }

            for (int monitorIndex = 0; monitorIndex < screen.NMonitors; monitorIndex++)
            {
                var geometry = screen.GetMonitorGeometry(monitorIndex);
                var mask = CreateMaskWindow(parent, opacity);
                mask.SetDefaultSize(geometry.Width, geometry.Height);
                mask.Move(geometry.X, geometry.Y);
                mask.Show();
                masks.Add(mask);
            }

            return masks;
        }

        private static Window CreateFullscreenMask(Window parent, float opacity)
        {
            var mask = CreateMaskWindow(parent, opacity);
            mask.Fullscreen();
            mask.Show();
            return mask;
        }

        private static Window CreateMaskWindow(Window parent, float opacity)
        {
            var mask = new Window(string.Empty);
            mask.TransientFor = parent;
            mask.ModifyBg(StateType.Normal, Color.Black.ToGdkColor());
            mask.Opacity = opacity;
            mask.CanFocus = false;
            mask.AcceptFocus = false;
            mask.Sensitive = false;
            mask.Decorated = false;
            return mask;
        }

        public static void ShowAll(IEnumerable<Window> masks)
        {
            if (masks == null)
            {
                return;
            }

            foreach (var mask in masks.Where(mask => mask != null))
            {
                mask.Show();
            }
        }

        public static void HideAll(IEnumerable<Window> masks)
        {
            if (masks == null)
            {
                return;
            }

            foreach (var mask in masks.Where(mask => mask != null && mask.Visible))
            {
                mask.Hide();
            }
        }

        public static void DestroyAll(IEnumerable<Window> masks)
        {
            if (masks == null)
            {
                return;
            }

            foreach (var mask in masks.Where(mask => mask != null))
            {
                mask.Destroy();
            }
        }
    }
}
