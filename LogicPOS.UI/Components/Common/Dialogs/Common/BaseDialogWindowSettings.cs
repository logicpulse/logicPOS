using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Settings;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Dialogs
{
    public class BaseDialogWindowSettings
    {
        public string Icon { get; set; }
        public System.Drawing.Size Size { get; set; }
        public EventBox CloseWindow { get; set; }
        public EventBox MinimizeWindow { get; set; }
        public Widget Content { get; set; }
        public Widget LeftContent { get; set; }
        public CustomButton ConfirmButton { get; set; }
        public ActionAreaButtons RightButtons { get; set; }
        public Window Source { get; set; }

        public int DragOffsetX { get; set; }
        public int DragOffsetY { get; set; }

        public uint BorderWidth => 5;

        private bool _showCloseButton = true;

        public bool ShowCloseButton

        {
            get { return _showCloseButton; }
            set
            {
                _showCloseButton = value;

                if (value)
                {
                    CloseWindow?.ShowAll();
                }
                else
                {
                    CloseWindow?.HideAll();
                }
            }
        }

        public bool UseMask = AppSettings.Instance.UseBaseDialogWindowMask;

        private readonly List<Window> _maskWindows = new List<Window>();

        public IReadOnlyList<Window> MaskWindows => _maskWindows;

        public Window Mask => _maskWindows.FirstOrDefault();

        public void SetMaskWindows(IEnumerable<Window> masks)
        {
            _maskWindows.Clear();
            if (masks == null)
            {
                return;
            }

            _maskWindows.AddRange(masks.Where(mask => mask != null));
        }

        public Label WindowTitle { get; set; } = new Label();
        public bool ConfirmDialogOnEnter { get; set; }
    }
}
