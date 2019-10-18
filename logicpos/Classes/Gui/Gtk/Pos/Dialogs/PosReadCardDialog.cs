using Gtk;
using logicpos.App;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Drawing;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosReadCardDialog : PosBaseDialog
    {
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        private EntryBoxValidation _entryBoxMovementDescription;

        private String _cardNumber;
        public String CardNumber
        {
            get { return _cardNumber; }
            set { _cardNumber = value; }
        }

        public PosReadCardDialog(Window pSourceWindow, DialogFlags pDialogFlags)
            : base(pSourceWindow, pDialogFlags)
        {
            //Settings
            String regexAlfaNumericExtended = SettingsApp.RegexAlfaNumericExtended;

            //Init Local Vars
            String windowTitle = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_readcard");
            Size windowSize = new Size(462, 320);//400 With Other Payments
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_read_card.png");

            //EntryDescription
            _entryBoxMovementDescription = new EntryBoxValidation(this, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_read_card"), KeyboardMode.AlfaNumeric, regexAlfaNumericExtended, false);
            //_entryBoxMovementDescription.EntryValidation.Changed += delegate { ValidateDialog(); };
            //VBox
            VBox vbox = new VBox(true, 0);
            //vbox.PackStart(_entryBoxMovementAmountOtherPayments, true, true, 0);
            vbox.PackStart(_entryBoxMovementDescription, true, true, 0);

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(vbox, 0, 0);

            //ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));

            _buttonOk.Clicked += _buttonOk_Clicked;

            this.KeyReleaseEvent += PosReadCardDialog_KeyReleaseEvent;
            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
        }

        void PosReadCardDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key.ToString().Equals("Return"))
            {
                CardNumber = _entryBoxMovementDescription.EntryValidation.Text;
                _buttonOk.Click();
            }
        }

        void _buttonOk_Clicked(object sender, EventArgs e)
        {
            CardNumber = _entryBoxMovementDescription.EntryValidation.Text;
        }
    }
}
