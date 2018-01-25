using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    //KeyboardKey UI Key Widgets
    public class KeyboardPadKey : TouchButtonBase
    {
        //Private Members
        private String _l1LabelText;
        private String _l2LabelText;
        private Color _colorKeyboardPadKeyDefaultFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorKeyboardPadKeyDefaultFont"]);
        private Color _colorKeyboardPadKeySecondaryFont = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorKeyboardPadKeySecondaryFont"]);
        private Color _colorKeyboardPadKeyBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorKeyboardPadKeyBackground"]);
        private Color _colorKeyboardPadKeyBackgroundActive = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorKeyboardPadKeyBackgroundActive"]);

        //Public Properties
        private Label _labelL1;
        public Label LabelL1
        {
            get { return _labelL1; }
            set { _labelL1 = value; }
        }
        private Label _labelL2;
        public Label LabelL2
        {
            get { return _labelL2; }
            set { _labelL2 = value; }
        }
        //Store VirtualKey in Properties
        public VirtualKey Properties { get; set; }
        //Active Modifier Key, Change Color
        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                if (_active)
                {
                    SetBackgroundColor(_colorKeyboardPadKeyBackgroundActive, _eventboxBackgroundColor);
                }
                else
                {
                    SetBackgroundColor(_colorKeyboardPadKeyBackground, _eventboxBackgroundColor);
                }
            }
        }

        //Constructor
        public KeyboardPadKey(VirtualKey virtualKey)
            : base("")
        {
            //Always Store full VirtualKey Properties in KeyBoardKey
            this.Properties = virtualKey;

            //Key Labels
            if (virtualKey.L1 != null) { _l1LabelText = virtualKey.L1.Glyph; } else { _l1LabelText = ""; };
            if (virtualKey.L2 != null) { _l2LabelText = virtualKey.L2.Glyph; } else { _l2LabelText = ""; };

            //Init Local Vars
            Size sizeKeyboardPadDefaultKey = Utils.StringToSize(GlobalFramework.Settings["sizeKeyboardPadDefaultKey"]);
            String fontKeyboardPadPrimaryKey = GlobalFramework.Settings["fontKeyboardPadPrimaryKey"];
            String fontKeyboardPadSecondaryKey = GlobalFramework.Settings["fontKeyboardPadSecondaryKey"];

            //ByPass Defaults
            if (virtualKey.L1.KeyWidth > 0)
            {
                sizeKeyboardPadDefaultKey.Width = virtualKey.L1.KeyWidth;
            };

            //Defaults
            //Prepare Pango Fonts      
            if (virtualKey.L1.IsBold) { fontKeyboardPadPrimaryKey = "Bold " + fontKeyboardPadPrimaryKey; };
            Pango.FontDescription fontDescriptionPrimaryKey = Pango.FontDescription.FromString(fontKeyboardPadPrimaryKey);
            Pango.FontDescription fontDescriptionSecondaryKey = Pango.FontDescription.FromString(fontKeyboardPadSecondaryKey);

            //Vbox
            VBox vbox = new VBox(true, 0);
            vbox.BorderWidth = 2;
            //Labels
            _labelL1 = new Label();
            _labelL1.Text = _l1LabelText;
            //Align
            switch (virtualKey.L1.HAlign)
            {
                case "left":
                    _labelL1.SetAlignment(0.00F, 0.5F);
                    break;
                case "right":
                    _labelL1.SetAlignment(1.00F, 0.5F);
                    break;
            }
            _labelL1.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(_colorKeyboardPadKeyDefaultFont));
            _labelL1.ModifyFont(fontDescriptionPrimaryKey);
            vbox.PackEnd(_labelL1);
            //HideL2 dont show L2
            if (_l2LabelText != string.Empty && !virtualKey.L1.HideL2)
            {
                _labelL2 = new Label();
                _labelL2.Text = _l2LabelText;
                _labelL1.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(_colorKeyboardPadKeyDefaultFont));
                _labelL2.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(_colorKeyboardPadKeySecondaryFont));
                _labelL1.ModifyFont(fontDescriptionSecondaryKey);
                _labelL2.ModifyFont(fontDescriptionSecondaryKey);
                _labelL1.SetAlignment(0.60F, 0.50F);
                _labelL2.SetAlignment(0.40F, 0.50F);
                vbox.PackStart(_labelL2);
            };

            //InitObject("", _colorKeyboardPadKeyBackground, vbox, sizeKeyboardPadDefaultKey.Width, sizeKeyboardPadDefaultKey.Height);

            //Changed for theme
            this.BorderWidth = 1;
            InitObject("", Color.Transparent, vbox, sizeKeyboardPadDefaultKey.Width, sizeKeyboardPadDefaultKey.Height);
        }
    }
}
