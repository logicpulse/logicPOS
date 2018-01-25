using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.shared;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class CheckButtonBox : EventBox
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Protected
        protected Pango.FontDescription _fontDescription;
        protected CheckButton _checkButton;
        //Public
        public bool Active
        {
            get { return _checkButton.Active; }
            set { _checkButton.Active = value; }
        }

        //Custom Event Handlers
        public event EventHandler Clicked;

        public CheckButtonBox(String pLabelText, bool pActive)
        {
            //Defaults
            String fontEntry = GlobalFramework.Settings["fontEntryBoxValue"];
            Color colorBaseDialogEntryBoxBackground = FrameworkUtils.StringToColor(GlobalFramework.Settings["colorBaseDialogEntryBoxBackground"]);

            int padding = 3;
            //This
            this.ModifyBg(StateType.Normal, Utils.ColorToGdkColor(colorBaseDialogEntryBoxBackground));
            this.BorderWidth = (uint)padding;
            //Font
            _fontDescription = Pango.FontDescription.FromString(fontEntry);
            //Construct CheckButton
            _checkButton = new CheckButton(pLabelText) { Active = pActive, BorderWidth =  (uint) padding };
            _checkButton.Child.ModifyFont(_fontDescription);
            _checkButton.Clicked += checkButton_Clicked;

            Add(_checkButton);
        }

        void checkButton_Clicked(object sender, EventArgs e)
        {
            if (Clicked != null) Clicked(sender, e);
        }
    }
}
