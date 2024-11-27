using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using System;

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

        public bool UseMask = AppSettings.Instance.useBaseDialogWindowMask;
        public Window Mask { get; set; }

        public Label WindowTitle { get; set; } = new Label();
        public bool ConfirmDialogOnEnter { get; set; }
    }
}
