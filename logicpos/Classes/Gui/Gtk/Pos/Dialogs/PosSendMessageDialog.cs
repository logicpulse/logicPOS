using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    class PosSendMessageDialog : PosInputTextDialog
    {
        //UI EntryBox
        private XPOEntryBoxSelectRecordValidation<SYS_UserDetail, TreeViewUser> _entryBoxSelectUser;
        private XPOEntryBoxSelectRecordValidation<POS_ConfigurationPlaceTerminal, TreeViewConfigurationPlaceTerminal> _entryBoxSelectTerminal;
        //Default Values
        private SYS_UserDetail _valueUser = null;
        public SYS_UserDetail ValueUser
        {
          get { return _valueUser; }
          set { _valueUser = value; }
        }

        private POS_ConfigurationPlaceTerminal _valueTerminal = null;
        public POS_ConfigurationPlaceTerminal ValueTerminal
        {
          get { return _valueTerminal; }
          set { _valueTerminal = value; }
        }

        public PosSendMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags)
        //public PosInputTextDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowTitle, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)            
            : base(pSourceWindow, pDialogFlags, Resx.window_title_dialog_send_message, "Label", "Default", SettingsApp.RegexAlfaNumericExtended, true)
        {
            this.HeightRequest = 320;

            //UserDetail
            CriteriaOperator criteriaOperatorUser = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectUser = new XPOEntryBoxSelectRecordValidation<SYS_UserDetail, TreeViewUser>(_sourceWindow, Resx.global_user, "Designation", "Oid", _valueUser, criteriaOperatorUser, SettingsApp.RegexGuid, false);
            _entryBoxSelectUser.EntryValidation.IsEditable = false;
            //Public Reference
            _valueUser = _entryBoxSelectUser.Value;

            //Terminal
            CriteriaOperator criteriaOperatorTerminal = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectTerminal = new XPOEntryBoxSelectRecordValidation<POS_ConfigurationPlaceTerminal, TreeViewConfigurationPlaceTerminal>(_sourceWindow, Resx.global_user, "Designation", "Oid", _valueTerminal, criteriaOperatorTerminal, SettingsApp.RegexGuid, false);
            _entryBoxSelectTerminal.EntryValidation.IsEditable = false;
            //Public Reference
            _valueTerminal = _entryBoxSelectTerminal.Value;

            _vbox.PackStart(_entryBoxSelectTerminal, true, true, 0);
            _vbox.PackStart(_entryBoxSelectUser, true, true, 0);
            _vbox.ShowAll();

            //dialog.VBoxContent.PackStart(textViewTouch, true, true, 0);
            //dialog.VBoxContent.ShowAll();

            /*
            //Init Local Vars
            String windowTitle = Resx.window_title_dialog_send_message;
            Size windowSize = new Size(600, 500);
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_send_message.png");

            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(new Label("Place content here"), 0, 0);

            //ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //ActionArea
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            actionAreaButtons.Add(new ActionAreaButton(buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(buttonCancel, ResponseType.Cancel));
            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, actionAreaButtons);
            */
        }
    }
}
