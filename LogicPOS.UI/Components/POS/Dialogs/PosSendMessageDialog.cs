using DevExpress.Data.Filtering;
using Gtk;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal class PosSendMessageDialog : PosInputTextDialog
    {
        //UI EntryBox
        private readonly XPOEntryBoxSelectRecordValidation<sys_userdetail, TreeViewUser> _entryBoxSelectUser;
        private readonly XPOEntryBoxSelectRecordValidation<pos_configurationplaceterminal, TreeViewConfigurationPlaceTerminal> _entryBoxSelectTerminal;

        public sys_userdetail ValueUser { get; set; } = null;

        public pos_configurationplaceterminal ValueTerminal { get; set; } = null;

        public PosSendMessageDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowIcon)
        //public PosInputTextDialog(Window pSourceWindow, DialogFlags pDialogFlags, string pWindowTitle, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)            
            : base(pSourceWindow, pDialogFlags, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_send_message"), pWindowIcon, "Label", "Default", LogicPOS.Utility.RegexUtils.RegexAlfaNumericEmail, true)
        {
            this.HeightRequest = 320;

            //UserDetail
            CriteriaOperator criteriaOperatorUser = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectUser = new XPOEntryBoxSelectRecordValidation<sys_userdetail, TreeViewUser>(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_user"), "Designation", "Oid", ValueUser, criteriaOperatorUser, LogicPOS.Utility.RegexUtils.RegexGuid, false);
            _entryBoxSelectUser.EntryValidation.IsEditable = false;
            //Public Reference
            ValueUser = _entryBoxSelectUser.Value;

            //Terminal
            CriteriaOperator criteriaOperatorTerminal = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _entryBoxSelectTerminal = new XPOEntryBoxSelectRecordValidation<pos_configurationplaceterminal, TreeViewConfigurationPlaceTerminal>(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_user"), "Designation", "Oid", ValueTerminal, criteriaOperatorTerminal, LogicPOS.Utility.RegexUtils.RegexGuid, false);
            _entryBoxSelectTerminal.EntryValidation.IsEditable = false;
            //Public Reference
            ValueTerminal = _entryBoxSelectTerminal.Value;

            _vbox.PackStart(_entryBoxSelectTerminal, true, true, 0);
            _vbox.PackStart(_entryBoxSelectUser, true, true, 0);
            _vbox.ShowAll();

            //dialog.VBoxContent.PackStart(textViewTouch, true, true, 0);
            //dialog.VBoxContent.ShowAll();

            /*
            //Init Local Vars
            String windowTitle = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_send_message;
            Size windowSize = new Size(600, 500);
            String fileDefaultWindowIcon = SharedUtils.OSSlash(GeneralSettings.Path["images"] + @"Icons\Windows\icon_window_send_message.png");

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
