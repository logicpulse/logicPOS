using Gtk;
using LogicPOS.UI.Extensions;
using System;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class CheckButtonBox : EventBox
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public CheckButtonBox(string pLabelText, bool pActive)
        {
            //Defaults
            string fontEntry = LogicPOS.Settings.AppSettings.Instance.fontEntryBoxValue;
            Color colorBaseDialogEntryBoxBackground = LogicPOS.Settings.AppSettings.Instance.colorBaseDialogEntryBoxBackground;

            int padding = 3;
            //This
            this.ModifyBg(StateType.Normal, colorBaseDialogEntryBoxBackground.ToGdkColor());
            this.BorderWidth = (uint)padding;
            //Font
            _fontDescription = Pango.FontDescription.FromString(fontEntry);
            //Construct CheckButton
            _checkButton = new CheckButton(pLabelText) { Active = pActive, BorderWidth = (uint)padding };
            _checkButton.Child.ModifyFont(_fontDescription);
            _checkButton.Clicked += checkButton_Clicked;

            Add(_checkButton);
        }

        private void checkButton_Clicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(sender, e);
        }
    }
}
